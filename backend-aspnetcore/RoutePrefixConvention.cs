using Microsoft.AspNetCore.Mvc.ApplicationModels;

public sealed class RoutePrefixConvention : IApplicationModelConvention
{
    private readonly AttributeRouteModel _prefix;

    public RoutePrefixConvention(string prefix)
    {
        _prefix = new AttributeRouteModel(new Microsoft.AspNetCore.Mvc.RouteAttribute(prefix));
    }

    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            foreach (var selector in controller.Selectors)
            {
                if (selector.AttributeRouteModel != null)
                {
                    selector.AttributeRouteModel = AttributeRouteModel.CombineAttributeRouteModel(
                        _prefix, selector.AttributeRouteModel);
                }
                else
                {
                    selector.AttributeRouteModel = _prefix;
                }
            }
        }
    }
}