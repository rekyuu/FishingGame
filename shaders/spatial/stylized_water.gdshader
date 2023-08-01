shader_type spatial;

// Waves from https://godotshaders.com/shader/stylized-water-with-depthfade/
// Water/foam from https://www.youtube.com/watch?v=PLCGL3RW548

uniform vec4 main_color: source_color;
uniform vec4 intersection_color: source_color;
uniform float intersection_max_threshold = 0.5;
uniform sampler2D displacement_guide: hint_default_white;
uniform float displacement_amount = 0.6;
uniform float near = 0.15;
uniform float far = 300.0;

uniform vec2 wave_strength = vec2(0.5, 0.25);
uniform vec2 wave_frequ = vec2(12.0, 12.0);
uniform vec2 time_factor = vec2(1.0, 2.0);

uniform sampler2D DEPTH_TEXTURE : hint_depth_texture, filter_linear_mipmap;

float linearize(float depth) {
	depth = 2.0 * depth - 1.0;
	return near * far / (far + depth * (near - far));
}

float waves(vec2 pos, float time) {
	return (wave_strength.y * sin(pos.y * wave_frequ.y + time * time_factor.y)) + (wave_strength.x * sin(pos.x * wave_frequ.x + time * time_factor.x));
}

void vertex() {
	VERTEX.y += waves(VERTEX.xy, TIME);
}

void fragment() {
	float depth = linearize(texture(DEPTH_TEXTURE, SCREEN_UV).x);
	float pos = linearize(FRAGCOORD.z);
	float diff = depth - pos;
	
	vec2 displacement = texture(displacement_guide, UV - TIME / 512.0).rg;
	displacement = ((displacement * 2.0) - 1.0) * displacement_amount;
	diff += displacement.x;
	
	vec4 col = mix(intersection_color, main_color, step(intersection_max_threshold, diff));
	ALBEDO = col.rgb;
}