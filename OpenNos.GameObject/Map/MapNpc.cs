using OpenNos.Core;
using OpenNos.Data;
using OpenNos.Domain;
using OpenNos.GameObject.Helpers;
using OpenNos.PathFinder;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace OpenNos.GameObject
{
    public class MapNpc : MapNpcDTO
    {
        #region Members

        public NpcMonster Npc;
        private int movetime;
        private Random random;

        #endregion

        #region Properties

        public bool EffectActivated { get; set; }

        public short FirstX { get; set; }

        public short FirstY { get; set; }

        public bool IsHostile { get; set; }

        public bool IsMate { get; set; }

        public bool IsOut { get; set; }

        public bool IsProtected { get; set; }

        public DateTime LastEffect { get; private set; }

        public DateTime LastMove { get; private set; }

        public IDisposable Life { get; private set; }

        public IDisposable LifeEvent { get; set; }

        public MapInstance MapInstance { get; set; }

        public ConcurrentBag<EventContainer> OnDeathEvents { get; set; }

        public List<Node> Path { get; set; }

        public List<Recipe> Recipes { get; set; }

        public Shop Shop { get; set; }

        public long Target { get; set; }

        public List<TeleporterDTO> Teleporters { get; set; }

        #endregion

        #region Methods

        public EffectPacket GenerateEff(int effectid)
        {
            return new EffectPacket
            {
                EffectType = 2,
                CharacterId = MapNpcId,
                Id = effectid
            };
        }

        public string GenerateIn()
        {
            var npcinfo = ServerManager.Instance.GetNpc(NpcVNum);
            if (npcinfo == null || IsDisabled)
            {
                return string.Empty;
            }

            IsOut = false;
            return $"in 2 {NpcVNum} {MapNpcId} {MapX} {MapY} {Position} 100 100 {Dialog} 0 0 -1 1 {(IsSitting ? 1 : 0)} -1 - 0 -1 0 0 0 0 0 0 0 0";
        }

        public string GenerateOut()
        {
            var npcinfo = ServerManager.Instance.GetNpc(NpcVNum);
            if (npcinfo == null || IsDisabled)
            {
                return string.Empty;
            }

            IsOut = true;
            return $"out 2 {MapNpcId}";
        }

        public string GenerateSay(string message, int type) => $"say 2 {MapNpcId} 2 {message}";

        public string GetNpcDialog() => $"npc_req 2 {MapNpcId} {Dialog}";

        public void Initialize(MapInstance currentMapInstance)
        {
            MapInstance = currentMapInstance;
            Initialize();
        }

        public override void Initialize()
        {
            random = new Random(MapNpcId);
            Npc = ServerManager.Instance.GetNpc(NpcVNum);
            LastEffect = DateTime.Now;
            LastMove = DateTime.Now;
            IsHostile = Npc.IsHostile;
            FirstX = MapX;
            EffectActivated = true;
            FirstY = MapY;
            EffectDelay = 4000;
            movetime = ServerManager.Instance.RandomNumber(500, 3000);
            Path = new List<Node>();
            Recipes = ServerManager.Instance.GetReceipesByMapNpcId(MapNpcId);
            Target = -1;
            Teleporters = ServerManager.Instance.GetTeleportersByNpcVNum((short)MapNpcId);
            var shop = ServerManager.Instance.GetShopByMapNpcId(MapNpcId);
            if (shop != null)
            {
                shop.Initialize();
                Shop = shop;
            }
        }

        public void RunDeathEvent()
        {
            MapInstance.InstanceBag.NpcsKilled++;
            OnDeathEvents.ToList().ForEach(e =>
            {
                if (e.EventActionType == EventActionType.THROWITEMS)
                {
                    Tuple<int, short, byte, int, int> evt = (Tuple<int, short, byte, int, int>)e.Parameter;
                    e.Parameter = new Tuple<int, short, byte, int, int>(MapNpcId, evt.Item2, evt.Item3, evt.Item4, evt.Item5);
                }

                EventHelper.Instance.RunEvent(e);
            });
            OnDeathEvents.Clear();
        }

        public void StartLife()
        {
            Life = Observable.Interval(TimeSpan.FromMilliseconds(400)).Subscribe(x =>
            {
                try
                {
                    if (!MapInstance.IsSleeping)
                    {
                        NpcLife();
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            });
        }

        /// <summary>
        /// Remove the current Target from Npc.
        /// </summary>
        internal void RemoveTarget()
        {
            if (Target != -1)
            {
                Path.Clear();
                Target = -1;

                // return to origin
                Path = BestFirstSearch.FindPath(new Node { X = MapX, Y = MapY }, new Node { X = FirstX, Y = FirstY }, MapInstance.Map.Grid);
            }
        }

        internal void StopLife()
        {
            Life?.Dispose();
            Life = null;
        }

        private string GenerateMv2() => $"mv 2 {MapNpcId} {MapX} {MapY} {Npc.Speed}";

        private void NpcLife()
        {
            var time = (DateTime.Now - LastEffect).TotalMilliseconds;
            if (time > EffectDelay)
            {
                if (IsMate || IsProtected)
                {
                    MapInstance.Broadcast(GenerateEff(825), MapX, MapY);
                }

                if (Effect > 0 && EffectActivated)
                {
                    MapInstance.Broadcast(GenerateEff(Effect), MapX, MapY);
                }

                LastEffect = DateTime.Now;
            }

            time = (DateTime.Now - LastMove).TotalMilliseconds;
            if (IsMoving && Npc.Speed > 0 && time > movetime)
            {
                movetime = ServerManager.Instance.RandomNumber(500, 3000);
                var point = (byte)ServerManager.Instance.RandomNumber(2, 4);
                var fpoint = (byte)ServerManager.Instance.RandomNumber(0, 2);

                var xpoint = (byte)ServerManager.Instance.RandomNumber(fpoint, point);
                var ypoint = (byte)(point - xpoint);

                var mapX = FirstX;
                var mapY = FirstY;

                if (MapInstance.Map.GetFreePosition(ref mapX, ref mapY, xpoint, ypoint))
                {
                    var value = (xpoint + ypoint) / (double)(2 * Npc.Speed);
                    Observable.Timer(TimeSpan.FromMilliseconds(1000 * value))
                      .Subscribe(
                          x =>
                          {
                              MapX = mapX;
                              MapY = mapY;
                          });

                    LastMove = DateTime.Now.AddSeconds(value);
                    MapInstance.Broadcast(new BroadcastPacket(null, GenerateMv2(), ReceiverType.All, xCoordinate: mapX, yCoordinate: mapY));
                }
            }

            if (Target == -1)
            {
                if (IsHostile && Shop == null)
                {
                    var monster = MapInstance.Monsters.FirstOrDefault(s => MapInstance == s.MapInstance && Map.GetDistance(new MapCell { X = MapX, Y = MapY }, new MapCell { X = s.MapX, Y = s.MapY }) < (Npc.NoticeRange > 5 ? Npc.NoticeRange / 2 : Npc.NoticeRange));
                    var session = MapInstance.Sessions.FirstOrDefault(s => MapInstance == s.Character.MapInstance && Map.GetDistance(new MapCell { X = MapX, Y = MapY }, new MapCell { X = s.Character.PositionX, Y = s.Character.PositionY }) < Npc.NoticeRange);

                    if (monster != null && session != null)
                    {
                        Target = monster.MapMonsterId;
                    }
                }
            }
            else if (Target != -1)
            {
                var monster = MapInstance.Monsters.FirstOrDefault(s => s.MapMonsterId == Target);
                if (monster == null || monster.CurrentHp < 1)
                {
                    Target = -1;
                    return;
                }

                NpcMonsterSkill npcMonsterSkill = null;
                if (ServerManager.Instance.RandomNumber(0, 10) > 8)
                {
                    npcMonsterSkill = Npc.Skills.Where(s => (DateTime.Now - s.LastSkillUse).TotalMilliseconds >= 100 * s.Skill.Cooldown).OrderBy(rnd => random.Next()).FirstOrDefault();
                }

                const short DAMAGE = 100;
                var distance = Map.GetDistance(new MapCell { X = MapX, Y = MapY }, new MapCell { X = monster.MapX, Y = monster.MapY });
                if (monster.CurrentHp > 0 && (npcMonsterSkill != null && distance < npcMonsterSkill.Skill.Range || distance <= Npc.BasicRange))
                {
                    if ((DateTime.Now - LastEffect).TotalMilliseconds >= 1000 + Npc.BasicCooldown * 200 && !Npc.Skills.Any() || npcMonsterSkill != null)
                    {
                        if (npcMonsterSkill != null)
                        {
                            npcMonsterSkill.LastSkillUse = DateTime.Now;
                            MapInstance.Broadcast($"ct 2 {MapNpcId} 3 {Target} {npcMonsterSkill.Skill.CastAnimation} {npcMonsterSkill.Skill.CastEffect} {npcMonsterSkill.Skill.SkillVNum}");
                        }

                        if (npcMonsterSkill != null && npcMonsterSkill.Skill.CastEffect != 0)
                        {
                            MapInstance.Broadcast(GenerateEff(Effect));
                        }

                        monster.CurrentHp -= DAMAGE;

                        MapInstance.Broadcast(npcMonsterSkill != null
                            ? $"su 2 {MapNpcId} 3 {Target} {npcMonsterSkill.SkillVNum} {npcMonsterSkill.Skill.Cooldown} {npcMonsterSkill.Skill.AttackAnimation} {npcMonsterSkill.Skill.Effect} 0 0 {(monster.CurrentHp > 0 ? 1 : 0)} {(int)((double)monster.CurrentHp / monster.Monster.MaxHP * 100)} {DAMAGE} 0 0"
                            : $"su 2 {MapNpcId} 3 {Target} 0 {Npc.BasicCooldown} 11 {Npc.BasicSkill} 0 0 {(monster.CurrentHp > 0 ? 1 : 0)} {(int)((double)monster.CurrentHp / monster.Monster.MaxHP * 100)} {DAMAGE} 0 0");

                        LastEffect = DateTime.Now;
                        if (monster.CurrentHp < 1)
                        {
                            RemoveTarget();
                            monster.IsAlive = false;
                            monster.LastMove = DateTime.Now;
                            monster.CurrentHp = 0;
                            monster.CurrentMp = 0;
                            monster.Death = DateTime.Now;
                            Target = -1;
                        }
                    }
                }
                else
                {
                    var maxdistance = Npc.NoticeRange > 5 ? Npc.NoticeRange / 2 : Npc.NoticeRange;
                    if (IsMoving)
                    {
                        const short MAX_DISTANCE = 5;
                        if (!Path.Any() && distance > 1 && distance < MAX_DISTANCE)
                        {
                            var xoffset = (short)ServerManager.Instance.RandomNumber(-1, 1);
                            var yoffset = (short)ServerManager.Instance.RandomNumber(-1, 1);

                            // go to monster
                            Path = BestFirstSearch.FindPath(new GridPos { X = MapX, Y = MapY }, new GridPos { X = (short)(monster.MapX + xoffset), Y = (short)(monster.MapY + yoffset) }, MapInstance.Map.Grid);
                        }

                        if (DateTime.Now > LastMove && Npc.Speed > 0 && Path.Any())
                        {
                            var maxindex = Path.Count > Npc.Speed / 2 && Npc.Speed > 1 ? Npc.Speed / 2 : Path.Count;
                            maxindex = maxindex < 1 ? 1 : maxindex;
                            var mapX = (short)Path.ElementAt(maxindex - 1).X;
                            var mapY = (short)Path.ElementAt(maxindex - 1).Y;
                            var waitingtime = Map.GetDistance(new MapCell { X = mapX, Y = mapY }, new MapCell { X = MapX, Y = MapY }) / (double)Npc.Speed;
                            MapInstance.Broadcast(new BroadcastPacket(null, $"mv 2 {MapNpcId} {mapX} {mapY} {Npc.Speed}", ReceiverType.All, xCoordinate: mapX, yCoordinate: mapY));
                            LastMove = DateTime.Now.AddSeconds(waitingtime > 1 ? 1 : waitingtime);

                            Observable.Timer(TimeSpan.FromMilliseconds((int)((waitingtime > 1 ? 1 : waitingtime) * 1000)))
                            .Subscribe(
                                x =>
                                {
                                    MapX = mapX;
                                    MapY = mapY;
                                });

                            Path.RemoveRange(0, maxindex > Path.Count ? Path.Count : maxindex);
                        }

                        if (Target != -1 && (MapId != monster.MapId || distance > MAX_DISTANCE))
                        {
                            RemoveTarget();
                        }
                    }
                }
            }
        }

        #endregion
    }
}