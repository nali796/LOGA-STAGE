/*{
  "DESCRIPTION": "Soie fluide : bandes de lumière déformées, reflets fins, halo central et anneau qui réagissent aux basses",
  "CREDIT": "LOGASTAGE - Conçu et créé par ILAN HANSBERGER",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "LOGASTAGE"],
  "INPUTS": [
    { "NAME": "scale", "LABEL": "Échelle de la soie", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "speed", "LABEL": "Vitesse", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "haloSize", "LABEL": "Taille du halo", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "grain", "LABEL": "Grain", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "bass", "LABEL": "Basses (à relier à l'audio)", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "c1", "LABEL": "Couleur 1", "TYPE": "color", "DEFAULT": [0.18, 0.05, 0.45, 1.0] },
    { "NAME": "c2", "LABEL": "Couleur 2", "TYPE": "color", "DEFAULT": [0.95, 0.20, 0.55, 1.0] },
    { "NAME": "c3", "LABEL": "Couleur 3", "TYPE": "color", "DEFAULT": [1.0, 0.78, 0.35, 1.0] }
  ]
}*/

// Soie fluide
// LOGASTAGE - Conçu et créé par ILAN HANSBERGER

float hsh(vec2 p) { return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453); }

float vnoise(vec2 p) {
    vec2 i = floor(p), f = fract(p);
    f = f * f * (3.0 - 2.0 * f);
    return mix(mix(hsh(i), hsh(i + vec2(1.0, 0.0)), f.x),
               mix(hsh(i + vec2(0.0, 1.0)), hsh(i + vec2(1.0)), f.x), f.y);
}

float fbm(vec2 p) {
    float s = 0.0, a = 0.5;
    for (int i = 0; i < 5; i++) {
        s += a * vnoise(p);
        p = p * 2.03 + vec2(1.7, 9.2);
        a *= 0.5;
    }
    return s;
}

void main() {
    vec2 fc = gl_FragCoord.xy;
    vec2 u = (fc - 0.5 * RENDERSIZE.xy) / RENDERSIZE.y;
    float t = TIME * (0.08 + speed * 0.35);
    float b = smoothstep(0.15, 0.9, bass);

    // soie : bandes fluides déformées
    vec2 p = u * (1.2 + scale * 2.0);
    vec2 q = vec2(fbm(p + t), fbm(p - t * 0.7 + 3.1));
    float s = fbm(p + 2.2 * q + vec2(t * 0.5, -t * 0.3));
    float band = sin(s * 9.0 + u.y * 2.0 - t * 2.0) * 0.5 + 0.5;
    band = pow(band, 3.0);

    vec3 col = mix(c1.rgb, c2.rgb, smoothstep(0.3, 0.7, q.x));
    col = mix(col, c3.rgb, smoothstep(0.45, 0.85, q.y));
    col *= band * (0.7 + 0.6 * s) + 0.06 * s;

    // reflets fins
    col += vec3(1.0) * pow(band, 18.0) * 0.25;

    // halo central qui respire avec la basse
    float r = length(u * vec2(1.0, 1.25));
    col += mix(c2.rgb, c3.rgb, 0.5) * exp(-r * r * (9.0 - haloSize * 6.0)) * (0.12 + b * 0.35);

    // anneau très fin au kick
    col += c3.rgb * smoothstep(0.004, 0.0, abs(r - 0.32 - b * 0.06)) * b * 0.5;

    // vignette, grain
    col *= smoothstep(1.4, 0.35, length(u));
    col += (hsh(fc + fract(TIME * 0.37)) - 0.5) * 0.05 * (0.4 + grain);

    col = 1.0 - exp(-col * 1.6);
    gl_FragColor = vec4(col, 1.0);
}
