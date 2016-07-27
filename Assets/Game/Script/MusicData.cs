public class MusicData {
    /// <summary>
    /// 此物件會獲得的分數
    /// </summary>
    public int iGainscore;
    /// <summary>
    /// 此物件所產生在哪個軌道編號
    /// </summary>
    public int iTrackIndex;
    public MusicData ( SpawnListData tSpawnListData)
    {
        MusicObjectData m_MusicObjectData =  DataPool.MusicObjectDT.MusicObjectDataList.Find
            ( obj => obj.Type == tSpawnListData.Type );
        iGainscore = m_MusicObjectData.Gainscore;
        iTrackIndex = tSpawnListData.Track;
    }
}
