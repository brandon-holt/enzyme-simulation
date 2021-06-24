close all; clear; clc;

valency = 1:5:100;
velocity = zeros(1, numel(valency));

counter = 1;
for v = valency
    
    particle_diameter = 30; % nm
    ny1 = 0;
    ny2 = 5000;
    ne = 200;
    Kon = 3;
    Koff = 1;
    Kcat = 3;
    time_step = .1; % s, resolution
    t_max = 400; % s, total time simulated
    
    [t, ~, ~, ~, ~, py2] = simulate_activity(ny1, ny2, ne, Kon, Koff, Kcat, v, particle_diameter, time_step, t_max);
    
    product = py2{1};
    half_max_index = find(product > max(product) / 2, 1, 'first');
    velocity(counter) = product(half_max_index) / t(half_max_index);
    
    counter = counter + 1;
    
end

figure; scatter(valency, velocity);
title('Velocity vs. Valency'); xlabel('Valency'); ylabel('Velocity');