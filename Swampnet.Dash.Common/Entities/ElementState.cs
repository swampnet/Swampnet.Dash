using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Swampnet.Dash.Common.Interfaces;
using Swampnet.Rules;

namespace Swampnet.Dash.Common.Entities
{
	/// <summary>
	/// A snapshot of data at a particular time
	/// </summary>
    public class ElementState : IContext
    {
        public ElementState()
        {
            Output = new List<Property>();
            Timestamp = DateTime.UtcNow;
        }

        public ElementState(string itemDefinitionId, object id)
            : this()
        {
			ElementId = itemDefinitionId;
			Id = id.ToString();
        }

		#region IContext
		/// <summary>
		/// Identifies this item within a dashboard
		/// </summary>
		/// <remarks>
		/// This could be a test definition id, or something else. It kind of doesn't matter as long as
		/// the client can use it to find this item on an update
		/// </remarks>
		public string Id { get; set; }

		/// <summary>
		/// What is Timestamp? Is it the last time the item was updated, or the time it was initially created. We kinda need both under different
		/// circumstances. (We might want to display the last updated but, at least in an argos style dash, we probably want to order it by created)
		/// </summary>
		public DateTime Timestamp { get; set; }

		public object GetParameterValue(string name)
		{
			object value = null;

			switch (name.ToLowerInvariant())
			{
				case "timestamp":
					value = Timestamp;
					break;

				case "status":
					value = Status;
					break;

				default:
					value = Output.Value(name);
					break;
			}

			return value;
		}
		#endregion

		/// <summary>
		/// Element id. Most of the time this will be the same as Id
		/// </summary>
		public string ElementId { get; set; }

		/// <summary>
		/// Sorting hint
		/// </summary>
		public string Order { get; set; }

        public Status Status { get; set; }
		public Status PreviousStatus { get; set; }

		public List<Property> Output { get; set; }

		public ElementState Copy()
		{
			return new ElementState()
			{
				Id = this.Id,
				ElementId = this.ElementId,
				Order = this.Order,
				Status = this.Status,
				PreviousStatus = this.PreviousStatus,
				Timestamp = this.Timestamp,
				Output = this.Output.Select(x => new Property() { Category = x.Category, Name = x.Name, Value = x.Value }).ToList()
			};
		}


		public override string ToString()
        {
            return $"{Status} (" + string.Join(", ", Output.Select(p => $"{p.Name}: {p.Value}")) + ")";
        }
	}
}
