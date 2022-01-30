Shader "Custom/Plant"
{
  Properties
  {
    _Color ("Color", Color) = (1,1,1,1)
    _MainTex ("Texture", 2D) = "white" {}
    _LightRamp ("Light Ramp", 2D) = "white" {}
    _WindDirection ("Wind Direction", Vector) = (.5, 0.3, 0.9, 0)
    _WindScale ("Wind Scale", float) = 0.25
    _WindFrequency ("Wind Frequency", float) = 5.0
    _SwayScale ("Sway Scale", float) = 0.25
    _WobbleFrequency ("Wobble Frequency", float) = 2.0
    _WeightScale ("Weight Scale", float) = 1.0
  }
  SubShader
  {
    Tags { "IgnoreProjector" = "True" }
    
    CGINCLUDE 
      #include "UnityCG.cginc"
      #include "CellShading.cginc"

      struct appdata
      {
        float4 vertex : POSITION;
        float3 normal : NORMAL;
        fixed4 color : COLOR;
        float2 uv : TEXCOORD0;
      };

      struct v2f
      {
        float4 pos : SV_POSITION;
        fixed4 color : COLOR;
        float2 uv : TEXCOORD0;
        SHADOW_COORDS(1)
        float3 worldNormal : TEXCOORD2;
        float4 worldPos : TEXCOORD3;
      };

      sampler2D _MainTex;
      float4 _Color;
      float4 _WindDirection;
      float _WindScale;
      float _WindFrequency;
      float _SwayScale;
      float _WobbleFrequency;
      float _WeightScale;

      v2f vert (appdata v)
      {
        v2f o;

        float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
        float windVariance = (sin(_Time.y * _WindFrequency + v.vertex.y * _WobbleFrequency + worldPos.x) * 0.5 + 1);
        float vertexWeight = v.vertex.y / _WeightScale;
        v.vertex += _WindDirection * ((_WindScale + windVariance * _SwayScale) * vertexWeight);
      
        o.pos = UnityObjectToClipPos(v.vertex);
        o.uv = v.uv;
        o.worldNormal = mul((float3x3)unity_ObjectToWorld, v.normal);
        o.worldPos = worldPos;
        o.color = v.color;

        TRANSFER_SHADOW(o)

        return o;
      }

      fixed4 frag (v2f i) : SV_Target
      {
        fixed lightAtten = 1;
        #ifdef POINT
        unityShadowCoord3 lightCoord = mul(unity_WorldToLight, unityShadowCoord4(i.worldPos, 1)).xyz;
        fixed shadow = UNITY_SHADOW_ATTENUATION(i, i.worldPos);
        lightAtten = tex2D(_LightTexture0, dot(lightCoord, lightCoord).rr).r * shadow;
        #endif

        // Get base diffuse color
        fixed4 diffuse = _Color * tex2D(_MainTex, i.uv);
        diffuse *= CalculateLighting(normalize(i.worldNormal), lightAtten, SHADOW_ATTENUATION(i));

        return diffuse;
      }
    ENDCG

    Pass 
    {
      Name "ShadowCaster"
      Tags { "LightMode" = "ShadowCaster" }
      ZWrite On 
      ZTest Less 
      Cull Back
      
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment shadowCastFrag

      float4 shadowCastFrag(v2f i) : COLOR
      {
        SHADOW_CASTER_FRAGMENT(i)
      }
      ENDCG
    }

    Pass
    {
      Tags { "RenderType"="Opaque" "LightMode" = "ForwardBase" }

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
      ENDCG
    }
  }

  FallBack "Diffuse"
}
