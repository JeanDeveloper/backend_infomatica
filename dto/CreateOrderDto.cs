namespace infomatica.dto
{
    public class CreateOrderDto
    {
        public DateTime OrderDate { get; set; }
        public List<CreateDetailOrderDto> DetailsOrder { get; set; }
    }
}
