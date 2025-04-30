Shader "Custom/SpriteFlash"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _FlashAmount ("Flash Amount", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True" "PreviewType"="Sprite" "CanUseSpriteAtlas"="True" }
        LOD 100

        Pass
        {
            Name "FORWARD"
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            Lighting Off
            ZWrite Off
            Fog { Mode Off }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            float _FlashAmount;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, i.texcoord);
                fixed4 flashColor = fixed4(1,1,1,1);
                return lerp(c, flashColor, _FlashAmount);
            }
            ENDCG
        }
    }
}
