namespace OpenNos.Data
{
    public interface IMapDTO
    {
        #region Properties

        byte[] Data { get; set; }

        short MapId { get; set; }

        int Music { get; set; }

        string Name { get; set; }

        bool ShopAllowed { get; set; }

        #endregion
    }
}