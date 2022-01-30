Shader "Custom/FishBowl"
{
  Properties
  {
    _WaterColor ("Water Color", Color) = (1,1,1,1)
    _WaterFoamColor ("Foam Color", Color) = (1,1,1,1)
    _WaterTopColor ("Foam Top Color", Color) = (1,1,1,1)
    _WaterFoamThreshold ("Foam Threshold", float) = 0.02
    _WaterInnerThreshold ("Top Threshold", float) = -0.06
    _WaterHeight ("Water Height", float) = 0.5
    _WaterSloshFreq ("Water Slosh Frequency", float) = 5
    _WaterSloshSpeed ("Water Slosh Speed", float) = 2
    _WaterSloshAmplitude ("Water Slosh Amplitude", float) = 0.03
  }
  SubShader
  {
    CGINCLUDE 
      #pragma target 3.0
      #include "UnityCG.cginc"

      struct appdata
      {
        float4 vertex : POSITION;
        fixed4 normal : NORMAL;
        fixed4 color : COLOR;
        float2 uv : TEXCOORD0;
      };

      struct v2f
      {
        float4 pos : SV_POSITION;
        fixed4 color : COLOR;
        float2 uv : TEXCOORD0;
        fixed3 worldNormal : TEXCOORD2;
        float4 worldPos : TEXCOORD3;
        float distToWater : TEXCOORD4;
        float waterOrigin : TEXCOORD5;
      };

      float4 _WaterColor;
      float4 _WaterFoamColor;
      float4 _WaterTopColor;
      float _WaterFoamWidth;
      float _WaterInnerThreshold;
      float _WaterFoamThreshold;
      float _WaterHeight;
      float _WaterSloshFreq;
      float _WaterSloshSpeed;
      float _WaterSloshAmplitude;

      v2f vert (appdata v)
      {
        v2f o;
        float animatedWaterHeight = _WaterHeight + sin(_Time.y * _WaterSloshSpeed + v.vertex.x * _WaterSloshFreq) * _WaterSloshAmplitude;
        float distToWater = v.vertex.y - animatedWaterHeight;
        float isAboveWater = step(_WaterHeight, v.vertex.y);
        v.vertex.y = lerp(v.vertex.y, animatedWaterHeight, isAboveWater);

        o.worldPos = mul(unity_ObjectToWorld, v.vertex);
        o.distToWater = distToWater;
        o.uv = v.uv;
        o.worldNormal = mul(unity_ObjectToWorld, fixed4(v.normal.xyz, 0));
        o.waterOrigin = mul(unity_ObjectToWorld, fixed4(0, 0, 0, 1)).y;
        o.pos = UnityWorldToClipPos(o.worldPos);

        return o;
      }

      fixed4 frag (v2f i, fixed facing : VFACE) : SV_Target
      {
        float innerWaterFactor = saturate(ceil(i.distToWater + _WaterInnerThreshold));
        float foamWaterFactor = saturate(ceil(i.distToWater + _WaterFoamThreshold));

        // rest of the liquid
        float4 result = _WaterColor;
        result = lerp(result, _WaterFoamColor, foamWaterFactor);
        result = lerp(result, _WaterTopColor, innerWaterFactor);

        fixed3 viewDir = normalize(i.worldPos - _WorldSpaceCameraPos);
        float fresnel = saturate(1.0 - abs(dot(viewDir, normalize(i.worldNormal))));
        result *= lerp(fresnel, 1, 0.5);


        //VFACE returns positive for front facing, negative for backfacing
        float4 diffuse = result;
        return diffuse;
      }
    ENDCG 

    Pass
    {
      Tags { "RenderType"="Transparent" "LightMode" = "ForwardBase" }

      Blend SrcAlpha OneMinusSrcAlpha
      Cull Front
      ZWrite On

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
      ENDCG
    }
  }

  Fallback "Diffuse"
}
