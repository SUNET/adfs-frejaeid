using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;
namespace Freja.Model
{
	public class Payload
	{
		public Payload()
		{
			attributesToReturn = new List<ReturnAttribute>();
		}
		public string userInfoType { get; set; }
		public string userInfo { get; set; }
		public List<ReturnAttribute> attributesToReturn { get; set; }
		public string minRegistrationLevel { get; set; }
	}

	public class ReturnAttribute
	{
		public string attribute { get; set; }
	}
	public class SsnUserInfo
	{
		public string country { get; set; }
		public string ssn { get; set; }
	}

	public class ShouldSerializeContractResolver : DefaultContractResolver
	{
		public static readonly ShouldSerializeContractResolver Instance = new ShouldSerializeContractResolver();

		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			JsonProperty property = base.CreateProperty(member, memberSerialization);

			if (property.PropertyType != typeof(string))
			{
				if (property.PropertyType.GetInterface(nameof(IEnumerable<object>)) != null)
					property.ShouldSerialize =
						instance => (instance?.GetType().GetProperty(property.PropertyName).GetValue(instance) as IEnumerable<object>)?.Count() > 0;
			}
			return property;
		}
	}
}
