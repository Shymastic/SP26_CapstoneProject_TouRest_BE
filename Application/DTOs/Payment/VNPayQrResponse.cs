namespace TouRest.Application.DTOs.Payment
{
    public class VNPayQrResponse
    {
        public string Code { get; set; } = "";
        public string Message { get; set; } = "";
        public string QrContent { get; set; } = "";
        public bool IsSuccess => Code == "00";
    }
}
