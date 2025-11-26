using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TinyToDo.Services;

namespace TinyToDo.Filters;

public class RequireAuthenticationAttribute: ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // <1> セッション情報が存在するかチェック
        var session = SessionService.Instance.CheckSession(
            context.HttpContext,
            out bool shouldRedirect
        );
        if (shouldRedirect || session == null)
        {
            context.Result = new RedirectResult("/login");
            return;
        }
       
        // <2> 認証済みかどうかをチェック
        if (session.UserAccount == null)
        {
            context.Result = new RedirectResult("/login");
            return;
        }
        
        // <3> 認証済みの場合、HttpContext.Itemsにセッションを格納
        context.HttpContext.Items["Session"] = session;
        base.OnActionExecuting(context);
    }
}