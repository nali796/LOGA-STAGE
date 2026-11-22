/*{
  "DESCRIPTION": "Anneaux de LED qui s'éloignent du centre en boucle",
  "CREDIT": "LOGASTAGE - Conçu et créé par ILAN HANSBERGER",
  "ISFVSN": "2",
  "CATEGORIES": ["Generator", "LED", "LOGASTAGE"],
  "INPUTS": [
    { "NAME": "density", "LABEL": "Densité de la grille", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "speed", "LABEL": "Vitesse des anneaux", "TYPE": "float", "DEFAULT": 0.5, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "palShift", "LABEL": "Décalage de palette", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "sharp", "LABEL": "Netteté des anneaux", "TYPE": "float", "DEFAULT": 0.4, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "bass", "LABEL": "Basses (à relier à l'audio)", "TYPE": "float", "DEFAULT": 0.0, "MIN": 0.0, "MAX": 1.0 },
    { "NAME": "dotSize", "LABEL": "Taille des LED", "TYPE": "float", "DEFAULT": 0.42, "MIN": 0.2, "MAX": 0.5 },
    { "NAME": "c1", "LABEL": "Couleur 1", "TYPE": "color", "DEFAULT": [1.0, 0.42, 0.0, 1.0] },
    { "NAME": "c2", "LABEL": "Couleur 2", "TYPE": "color", "DEFAULT": [0.93, 0.04, 0.47, 1.0] },
    { "NAME": "c3", "LABEL": "Couleur 3", "TYPE": "color", "DEFAULT": [1.0, 0.82, 0.0, 1.0] }
  ]
}*/

// Anneaux de LED
// LOGASTAGE - Conçu et créé par ILAN HANSBERGER

vec3 pal3(float t) {
    t = fract(t) * 3.0;
    if (t < 1.0) return mix(c1.rgb, c2.rgb, t);
    if (t < 2.0) return mix(c2.rgb, c3.rgb, t - 1.0);
    return mix(c3.rgb, c1.rgb, t - 2.0);
}

// anneaux concentriques en boucle (période 2*PI/5 : boucle sans coupure)
float eff(vec2 c, float t) {
    float loopT = mod(t, 6.2831853 / 5.0);
    return 0.5 + 0.5 * sin(length(c) * 0.7 - loopT * 5.0);
}

void main() {
    vec2 u = gl_FragCoord.xy / RENDERSIZE.y;
    float t = TIME * (0.5 + speed * 2.0);

    // grille de LED : 8 à 32 cases en hauteur
    float n = 8.0 + floor(density * 24.0);
    vec2 g = u * n;
    vec2 id = floor(g);
    vec2 q = fract(g) - 0.5;
    vec2 c = id - vec2(RENDERSIZE.x / RENDERSIZE.y, 1.0) * n * 0.5;

    float v = eff(c, t);
    v = pow(max(v, 0.0), 2.0 + sharp * 4.0);

    // forme ronde de chaque LED
    float dt = smoothstep(dotSize, dotSize - 0.12, length(q));

    float palPhase = id.y / n * 0.5 + mod(t * 0.05, 1.0) + palShift;
    vec3 col = pal3(palPhase) * v * dt * (0.7 + bass * 0.6) + vec3(0.02) * dt;
    gl_FragColor = vec4(col, 1.0);
}
