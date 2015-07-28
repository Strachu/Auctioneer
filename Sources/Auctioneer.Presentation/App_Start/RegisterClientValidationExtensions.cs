using DataAnnotationsExtensions.ClientValidation;

[assembly: WebActivator.PreApplicationStartMethod(typeof(Auctioneer.Presentation.App_Start.RegisterClientValidationExtensions), "Start")]
 
namespace Auctioneer.Presentation.App_Start {
    public static class RegisterClientValidationExtensions {
        public static void Start() {
            DataAnnotationsModelValidatorProviderExtensions.RegisterValidationExtensions();            
        }
    }
}