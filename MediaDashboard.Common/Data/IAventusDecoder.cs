namespace MediaDashboard.Common.Data
{
    public interface IAventusObject
    {
        AventusHealth Health { get; set; }
    }
    public interface IAventusCoder : IAventusObject
    {
        long BitRate { get; set; }
        long ExpectedBitRate { get; set; }
        int TaskIndex { get; set; }

        string TaskGroupId { get; set; }

        
    }
    public interface IAventusEncoder : IAventusCoder
    {
        string EncoderName { get; set; }
    }
    public interface IAventusDecoder : IAventusCoder
    {

        string Codec { get; set; }

        int Pid { get; set; }

       long TotalLostDuration { get; set; }
    }

    public interface ICodecConfig
    {
        string EncoderType { get; set; }
    }
}
