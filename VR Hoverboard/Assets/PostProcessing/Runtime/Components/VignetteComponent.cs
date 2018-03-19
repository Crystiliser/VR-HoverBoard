namespace UnityEngine.PostProcessing
{
    public sealed class VignetteComponent : PostProcessingComponentRenderTexture<VignetteModel>
    {
        private static class Uniforms
        {
            internal static readonly int _Vignette_Color = Shader.PropertyToID("_Vignette_Color");
            internal static readonly int _Vignette_Center = Shader.PropertyToID("_Vignette_Center");
            internal static readonly int _Vignette_Settings = Shader.PropertyToID("_Vignette_Settings");
            internal static readonly int _Vignette_Mask = Shader.PropertyToID("_Vignette_Mask");
            internal static readonly int _Vignette_Opacity = Shader.PropertyToID("_Vignette_Opacity");
        }
        public override bool active => model.enabled && !context.Interrupted;
        public override void Prepare(Material uberMaterial)
        {
            VignetteModel.Settings settings = model.GetSettings;
            uberMaterial.SetColor(Uniforms._Vignette_Color, settings.color);
            if (VignetteModel.Mode.Classic == settings.mode)
            {
                uberMaterial.SetVector(Uniforms._Vignette_Center, settings.center);
                uberMaterial.EnableKeyword("VIGNETTE_CLASSIC");
                uberMaterial.SetVector(Uniforms._Vignette_Settings, new Vector4(settings.intensity * 3.0f, settings.smoothness * 5.0f, (1.0f - settings.roundness) * 6.0f + settings.roundness, settings.rounded ? 1.0f : 0.0f));
            }
            else if (VignetteModel.Mode.Masked == settings.mode)
            {
                if (null != settings.mask && settings.opacity > 0.0f)
                {
                    uberMaterial.EnableKeyword("VIGNETTE_MASKED");
                    uberMaterial.SetTexture(Uniforms._Vignette_Mask, settings.mask);
                    uberMaterial.SetFloat(Uniforms._Vignette_Opacity, settings.opacity);
                }
            }
        }
    }
}