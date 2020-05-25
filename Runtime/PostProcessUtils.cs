namespace UnityEngine.Rendering.Universal
{
    public static class PostProcessUtils
    {
        [System.Obsolete("This method is obsolete. Use ConfigureDithering override that takes camera pixel width and height instead.")]
        public static int ConfigureDithering(PostProcessData data, int index, Camera camera, Material material)
        {
            return ConfigureDithering(data, index, camera.pixelWidth, camera.pixelHeight, material);
        }

        // TODO: Add API docs
        public static int ConfigureDithering(PostProcessData data, int index, int cameraPixelWidth, int cameraPixelHeight, Material material)
        {
            var blueNoise = data.textures.blueNoise16LTex;

            if (blueNoise == null || blueNoise.Length == 0)
                return 0; // Safe guard

            #if LWRP_DEBUG_STATIC_POSTFX // Used by QA for automated testing
            index = 0;
            float rndOffsetX = 0f;
            float rndOffsetY = 0f;
            #else
            if (++index >= blueNoise.Length)
                index = 0;

            float rndOffsetX = Random.value;
            float rndOffsetY = Random.value;
            #endif

            // Ideally we would be sending a texture array once and an index to the slice to use
            // on every frame but these aren't supported on all Universal targets
            var noiseTex = blueNoise[index];

            material.SetTexture(ShaderConstants._BlueNoise_Texture, noiseTex);
            material.SetVector(ShaderConstants._Dithering_Params, new Vector4(
                cameraPixelWidth / (float)noiseTex.width,
                cameraPixelHeight / (float)noiseTex.height,
                rndOffsetX,
                rndOffsetY
            ));

            return index;
        }
        

      
        // Precomputed shader ids to same some CPU cycles (mostly affects mobile)
        static class ShaderConstants
        {
            public static readonly int _Grain_Texture = Shader.PropertyToID("_Grain_Texture");
            public static readonly int _Grain_Params = Shader.PropertyToID("_Grain_Params");
            public static readonly int _Grain_TilingParams = Shader.PropertyToID("_Grain_TilingParams");

            public static readonly int _BlueNoise_Texture = Shader.PropertyToID("_BlueNoise_Texture");
            public static readonly int _Dithering_Params  = Shader.PropertyToID("_Dithering_Params");
        }
    }
}
