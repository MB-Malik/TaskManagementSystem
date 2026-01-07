using System.ComponentModel.DataAnnotations;

namespace TskMngmntSys.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}