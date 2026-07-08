using Yarp.ReverseProxy.Transforms.Builder;

namespace APIGateaway.API.Infrastructure
{
    public class EmptyTransformFactory : ITransformFactory
    {
        public bool Validate(TransformRouteValidationContext context, IReadOnlyDictionary<string, string> transformValues) => true;

        public bool Build(TransformBuilderContext context, IReadOnlyDictionary<string, string> transformValues) => true;
    }
}
