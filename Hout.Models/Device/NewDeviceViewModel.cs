using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hout.Models.Specifications;
using Hout.Models.Specifications.ViewModels;
using Newtonsoft.Json;

namespace Hout.Models.Device
{
    public class NewDeviceViewModel
    {
        public NewDeviceViewModel(string name, string description, Dictionary<string, object> properties, NameDescCollection<PropertySpecification> propertySpecifications,
            Type explorerType)
        {
            Name = name;
            Description = description;
            Properties = properties;
            ExplorerType = explorerType;
            PropertySpecifications = propertySpecifications.Select(s => new PropertySpecViewModel(s.Value)).ToArray();
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public Dictionary<string, object> Properties { get; set; }
        public PropertySpecViewModel[] PropertySpecifications { get; set; }
        [JsonIgnore]
        public Type ExplorerType { get; set; }
        public string Type => ExplorerType.FullName;
    }
}
