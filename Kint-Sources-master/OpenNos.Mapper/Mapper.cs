using OpenNos.Mapper.Mappers;

namespace OpenNos.Mapper
{
    public class Mapper
    {
        private AccountMapper _accountMapper;

        private BazaarItemMapper _bazaarItemMapper;

        private BCardMapper _bCardMapper;

        private ItemInstanceMapper _boxItemMapper;

        private CardMapper _cardMapper;

        private CellonOptionMapper _cellonOptionMapper;

        private CharacterMapper _characterMapper;

        private CharacterRelationMapper _characterRelationMapper;

        private CharacterSkillMapper _characterSkillMapper;

        private ComboMapper _comboMapper;

        private DropMapper _dropMapper;

        private FamilyCharacterMapper _familyCharacterMapper;

        private FamilyLogMapper _familyLogMapper;

        private FamilyMapper _familyMapper;

        private GeneralLogMapper _generalLogMapper;

        private ItemInstanceMapper _itemInstanceMapper;

        private ItemMapper _itemMapper;

        private MailMapper _mailMapper;

        private MaintenanceLogMapper _maintenanceLogMapper;

        private MapMapper _mapMapper;

        private MapMonsterMapper _mapMonsterMapper;

        private MapNPCMapper _mapNPCMapper;

        private MapTypeMapMapper _mapTypeMapMapper;

        private MapTypeMapper _mapTypeMapper;

        private MateMapper _mateMapper;

        private MinilandObjectMapper _minilandObjectMapper;

        private NpcMonsterMapper _npcMonsterMapper;

        private NpcMonsterSkillMapper _npcMonsterSkillMapper;

        private PenaltyLogMapper _penaltyLogMapper;

        private PortalMapper _portalMapper;

        private QuestMapper _questMapper;

        private QuestProgressMapper _questProgressMapper;

        private QuicklistEntryMapper _quicklistEntryMapper;

        private RecipeItemMapper _recipeItemMapper;

        private RecipeListMapper _recipeListMapper;

        private RecipeMapper _recipeMapper;

        private RespawnMapper _respawnMapper;

        private RespawnMapTypeMapper _respawnMapTypeMapper;

        private RollGeneratedItemMapper _rollGeneratedItemMapper;

        private ScriptedInstanceMapper _scriptedInstanceMapper;

        private ShellEffectMapper _shellEffectMapper;

        private ShopItemMapper _shopItemMapper;

        private ShopMapper _shopMapper;

        private ShopSkillMapper _shopSkillMapper;

        private SkillMapper _skillMapper;

        private StaticBonusMapper _staticBonusMapper;

        private StaticBuffMapper _staticBuffMapper;

        private TeleporterMapper _teleporterMapper;

        private static Mapper _instance;

        public Mapper()
        {
            _accountMapper = new AccountMapper();
            _bazaarItemMapper = new BazaarItemMapper();
            _bCardMapper = new BCardMapper();
            _boxItemMapper = new ItemInstanceMapper();
            _cardMapper = new CardMapper();
            _cellonOptionMapper = new CellonOptionMapper();
            _characterMapper = new CharacterMapper();
            _characterRelationMapper = new CharacterRelationMapper();
            _characterSkillMapper = new CharacterSkillMapper();
            _comboMapper = new ComboMapper();
            _dropMapper = new DropMapper();
            _familyCharacterMapper = new FamilyCharacterMapper();
            _familyLogMapper = new FamilyLogMapper();
            _familyMapper = new FamilyMapper();
            _generalLogMapper = new GeneralLogMapper();
            _itemInstanceMapper = new ItemInstanceMapper();
            _itemMapper = new ItemMapper();
            _mailMapper = new MailMapper();
            _maintenanceLogMapper = new MaintenanceLogMapper();
            _mapMapper = new MapMapper();
            _mapMonsterMapper = new MapMonsterMapper();
            _mapNPCMapper = new MapNPCMapper();
            _mapTypeMapMapper = new MapTypeMapMapper();
            _mapTypeMapper = new MapTypeMapper();
            _mateMapper = new MateMapper();
            _minilandObjectMapper = new MinilandObjectMapper();
            _npcMonsterMapper = new NpcMonsterMapper();
            _npcMonsterSkillMapper = new NpcMonsterSkillMapper();
            _penaltyLogMapper = new PenaltyLogMapper();
            _portalMapper = new PortalMapper();
            _questMapper = new QuestMapper();
            _questProgressMapper = new QuestProgressMapper();
            _quicklistEntryMapper = new QuicklistEntryMapper();
            _recipeItemMapper = new RecipeItemMapper();
            _recipeListMapper = new RecipeListMapper();
            _recipeMapper = new RecipeMapper();
            _respawnMapper = new RespawnMapper();
            _respawnMapTypeMapper = new RespawnMapTypeMapper();
            _rollGeneratedItemMapper = new RollGeneratedItemMapper();
            _scriptedInstanceMapper = new ScriptedInstanceMapper();
            _shellEffectMapper = new ShellEffectMapper();
            _shopItemMapper = new ShopItemMapper();
            _shopMapper = new ShopMapper();
            _shopSkillMapper = new ShopSkillMapper();
            _skillMapper = new SkillMapper();
            _staticBonusMapper = new StaticBonusMapper();
            _staticBuffMapper = new StaticBuffMapper();
            _teleporterMapper = new TeleporterMapper();
        }

        public static Mapper Instance => _instance ?? (_instance = new Mapper());

        public AccountMapper AccountMapper => _accountMapper;

        public BazaarItemMapper BazaarItemMapper => _bazaarItemMapper;

        public BCardMapper BCardMapper => _bCardMapper;

        public ItemInstanceMapper BoxItemMapper => _boxItemMapper;

        public CardMapper CardMapper => _cardMapper;

        public CellonOptionMapper CellonOptionMapper => _cellonOptionMapper;

        public CharacterMapper CharacterMapper => _characterMapper;

        public CharacterRelationMapper CharacterRelationMapper => _characterRelationMapper;

        public CharacterSkillMapper CharacterSkillMapper => _characterSkillMapper;

        public ComboMapper ComboMapper => _comboMapper;

        public DropMapper DropMapper => _dropMapper;

        public FamilyCharacterMapper FamilyCharacterMapper => _familyCharacterMapper;

        public FamilyLogMapper FamilyLogMapper => _familyLogMapper;

        public FamilyMapper FamilyMapper => _familyMapper;

        public GeneralLogMapper GeneralLogMapper => _generalLogMapper;

        public ItemInstanceMapper ItemInstanceMapper => _itemInstanceMapper;

        public ItemMapper ItemMapper => _itemMapper;

        public MailMapper MailMapper => _mailMapper;

        public MaintenanceLogMapper MaintenanceLogMapper => _maintenanceLogMapper;

        public MapMapper MapMapper => _mapMapper;

        public MapMonsterMapper MapMonsterMapper => _mapMonsterMapper;

        public MapNPCMapper MapNPCMapper => _mapNPCMapper;

        public MapTypeMapMapper MapTypeMapMapper => _mapTypeMapMapper;

        public MapTypeMapper MapTypeMapper => _mapTypeMapper;

        public MateMapper MateMapper => _mateMapper;

        public MinilandObjectMapper MinilandObjectMapper => _minilandObjectMapper;

        public NpcMonsterMapper NpcMonsterMapper => _npcMonsterMapper;

        public NpcMonsterSkillMapper NpcMonsterSkillMapper => _npcMonsterSkillMapper;

        public PenaltyLogMapper PenaltyLogMapper => _penaltyLogMapper;

        public PortalMapper PortalMapper => _portalMapper;

        public QuestMapper QuestMapper => _questMapper;

        public QuestProgressMapper QuestProgressMapper => _questProgressMapper;

        public QuicklistEntryMapper QuicklistEntryMapper => _quicklistEntryMapper;

        public RecipeItemMapper RecipeItemMapper => _recipeItemMapper;

        public RecipeListMapper RecipeListMapper => _recipeListMapper;

        public RecipeMapper RecipeMapper => _recipeMapper;

        public RespawnMapper RespawnMapper => _respawnMapper;

        public RespawnMapTypeMapper RespawnMapTypeMapper => _respawnMapTypeMapper;

        public RollGeneratedItemMapper RollGeneratedItemMapper => _rollGeneratedItemMapper;

        public ScriptedInstanceMapper ScriptedInstanceMapper => _scriptedInstanceMapper;

        public ShellEffectMapper ShellEffectMapper => _shellEffectMapper;

        public ShopItemMapper ShopItemMapper => _shopItemMapper;

        public ShopMapper ShopMapper => _shopMapper;

        public ShopSkillMapper ShopSkillMapper => _shopSkillMapper;

        public SkillMapper SkillMapper => _skillMapper;

        public StaticBonusMapper StaticBonusMapper => _staticBonusMapper;

        public StaticBuffMapper StaticBuffMapper => _staticBuffMapper;

        public TeleporterMapper TeleporterMapper => _teleporterMapper;
    }
}
