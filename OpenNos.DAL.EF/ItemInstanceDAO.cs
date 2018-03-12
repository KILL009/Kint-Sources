using AutoMapper;
using OpenNos.Core;
using OpenNos.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace OpenNos.DAL.EF
{
    public class ItemInstanceDAO<TEntity, TDTO> : GenericDAO<TEntity, TDTO> where TEntity : class
    {
        #region Methods

        public override void InitializeMapper()
        {
            // avoid override of mapping
        }

        public void InitializeMapper(Type baseType)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap(baseType, typeof(ItemInstance))
                    .ForMember("Item", opts => opts.Ignore());

                cfg.CreateMap(typeof(ItemInstance), typeof(ItemInstanceDTO)).As(baseType);

                Type itemInstanceType = typeof(ItemInstance);
                foreach (KeyValuePair<Type, Type> entry in Mappings)
                {
                    // GameObject -> Entity
                    cfg.CreateMap(entry.Key, entry.Value).ForMember("Item", opts => opts.Ignore())
                                    .IncludeBase(baseType, typeof(ItemInstance));

                    // Entity -> GameObject
                    cfg.CreateMap(entry.Value, entry.Key)
                                    .IncludeBase(typeof(ItemInstance), baseType);

                    // Entity -> GameObject
                    cfg.CreateMap(entry.Value, typeof(ItemInstanceDTO)).As(entry.Key);
                }
            });

            Mapper = config.CreateMapper();
        }

        public override IMappingBaseDAO RegisterMapping(Type gameObjectType)
        {
            try
            {
                Type targetType = Assembly.GetExecutingAssembly().GetTypes().SingleOrDefault(t => t.Name.Equals(gameObjectType.Name));
                Mappings.Add(gameObjectType, targetType);

                foreach (PropertyInfo pi in gameObjectType.GetProperties())
                {
                    object[] attrs = pi.GetCustomAttributes(typeof(KeyAttribute), false);
                    if (attrs.Length != 1)
                    {
                        continue;
                    }
                    PrimaryKey = pi;
                    break;
                }
                if (PrimaryKey != null)
                {
                    return this;
                }
                throw new KeyNotFoundException();
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return null;
            }
        }

        #endregion
    }
}