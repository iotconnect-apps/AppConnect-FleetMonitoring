using System.ComponentModel.DataAnnotations;

namespace IoTConnect.Model
{
   public class CommandExecuteModel
    {
        /// <summary>
        /// Command Guid. Call Template.AllTemplateCommand() method to get commandGuid.
        /// </summary>
        [Required(ErrorMessage = "CommandGuid is required")]
        public string commandGuid { get; set; }

        /// <summary>
        /// Param Value.
        /// </summary>
        public string parameterValue { get; set; }
    }
}
