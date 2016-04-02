namespace Hout.Models.Specifications
{
    public class EventSpecification : INameDesc
    {
        public EventSpecification()
        {
            ParameterSpecifications = new NameDescCollection<ParameterSpecification>();
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public NameDescCollection<ParameterSpecification> ParameterSpecifications { get; set; }
    }
}
