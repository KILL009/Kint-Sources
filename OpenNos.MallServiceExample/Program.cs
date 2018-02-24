using System;
using System.Linq;
using System.Text;
using OpenNos.Master.Library.Client;
using System.Configuration;
using System.Security.Cryptography;
using OpenNos.Master.Library.Data;
using OpenNos.Data;
using OpenNos.Domain;
using System.Collections.Generic;
using System.Threading;

namespace OpenNos.MallServiceExample
{
    class Program
    {
        static void Main(string[] args)
        {
            MallServiceClient.Instance.Authenticate(ConfigurationManager.AppSettings["MasterAuthKey"]);
            //Console.WriteLine("UserName:");
            //string user = Console.ReadLine();
            //Console.WriteLine("Password:");
            //string pass = Console.ReadLine();

            //pass = Sha512(pass);

            //AccountDTO account = MallServiceClient.Instance.ValidateAccount(user, pass);
            ////if(account != null && account.Authority > AuthorityType.Unconfirmed)
            ////{
            //IEnumerable<CharacterDTO> characters = MallServiceClient.Instance.GetCharacters(1);

            //foreach (CharacterDTO character in characters)
            //{
            //    Console.WriteLine($"ID: {character.CharacterId} Name: {character.Name} Level: {character.Level} Class: {character.Class}");
            //}
            Console.WriteLine("CharacterID:");
            long charId = long.Parse(Console.ReadLine());

            //if (characters.Any(s => s.CharacterId == charId))
            //{
            Console.WriteLine("ItemVNum:");
            short vnum = short.Parse(Console.ReadLine());
            Console.WriteLine("Amount:");
            byte amount = byte.Parse(Console.ReadLine());
            Console.WriteLine("Rare:");
            byte rare = byte.Parse(Console.ReadLine());
            Console.WriteLine("Upgrade:");
            byte upgrade = byte.Parse(Console.ReadLine());

            MallServiceClient.Instance.SendItem(charId, new MallItem()
            {
                ItemVNum = vnum,
                Amount = amount,
                Rare = rare,
                Upgrade = upgrade
            });
            //}
            //}
        }

        static string Sha512(string inputString)
        {
            using (SHA512 hash = SHA512.Create())
            {
                return string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(inputString)).Select(item => item.ToString("x2")));
            }
        }
    }
}
