
namespace System.Web.Mvc
{
    public class ResourceNotFoundResult : ActionResult
    {
        public string Message { 
            get; 
            set; 
        }

        public ResourceNotFoundResult(string message = "")
        {
            Message = message;
        }
        
        public override void ExecuteResult(ControllerContext context) {
            context.Controller.TempData["ErrorMessage"] = Message;
            throw new HttpException(404, Message);
        }
    }
}
