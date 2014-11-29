using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RequirementGathering.Reousrces;

namespace RequirementGathering.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldRequired")]
        [Display(Name = "EmailDisplay", ResourceType = typeof(Resources))]
        [EmailAddress(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "EmailInvalid", ErrorMessage = null)]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldRequired")]
        public string Provider { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldRequired")]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldRequired")]
        [Display(Name = "EmailDisplay", ResourceType = typeof(Resources))]
        [EmailAddress(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "EmailInvalid", ErrorMessage = null)]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldRequired")]
        [Display(Name = "EmailDisplay", ResourceType = typeof(Resources))]
        [EmailAddress(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "EmailInvalid", ErrorMessage = null)]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldRequired")]
        [Display(Name = "PasswordDisplay", ResourceType = typeof(Resources))]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "RememberDisplay", ResourceType = typeof(Resources))]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldRequired")]
        [EmailAddress(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "EmailInvalid", ErrorMessage = null)]
        [Display(Name = "EmailDisplay", ResourceType = typeof(Resources))]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldRequired")]
        [MinLength(2, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldLengthMin")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldLengthMax")]
        [Display(Name = "FirstNameDisplay", ResourceType = typeof(Resources))]
        public string FirstName { get; set; }

        [MaxLength(50, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldLengthMax")]
        [Display(Name = "LastNameDisplay", ResourceType = typeof(Resources))]
        public string LastName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldRequired")]
        [MinLength(6, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldLengthMin")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldLengthMax")]
        [DataType(DataType.Password)]
        [Display(Name = "PasswordDisplay", ResourceType = typeof(Resources))]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPasswordDisplay", ResourceType = typeof(Resources))]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        private List<string> _roles;
        [Display(Name = "RolesDisplay", ResourceType = typeof(Resources))]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public List<string> Roles
        {
            get
            {
                return _roles ?? new List<string> { "Administrator", "Researcher" };
            }
            set { _roles = value; }
        }
    }

    public class ResetPasswordViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldRequired")]
        [EmailAddress(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "EmailInvalid", ErrorMessage = null)]
        [Display(Name = "EmailDisplay", ResourceType = typeof(Resources))]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldRequired")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "FieldRequired")]
        [EmailAddress(ErrorMessageResourceType = typeof(Resources), ErrorMessageResourceName = "EmailInvalid", ErrorMessage = null)]
        [Display(Name = "EmailDisplay", ResourceType = typeof(Resources))]
        public string Email { get; set; }
    }
}
