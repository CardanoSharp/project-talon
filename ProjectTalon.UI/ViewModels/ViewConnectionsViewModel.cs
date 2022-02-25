using System.Runtime.Serialization;

namespace ProjectTalon.UI.ViewModels;

[DataContract]
public class ViewConnectionsViewModel: ViewModelBase
{
    [DataMember] public string Name { get; set; }
}