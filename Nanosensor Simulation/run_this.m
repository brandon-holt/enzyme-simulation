clear; clc; close all;

% initialize parameters
valency = 40; % sensors per nanoparticle
particle_diameter = 30; % nm

ny1 = 0;
ny2 = valency*100;
ne = 200;
Kon = 3;
Koff = 1;
Kcat = 3;
time_step = 1; % s, resolution
t_max = 400; % s, total time simulated

% generate particle graph
GenerateGraph(valency, 8);
title('Nanosensor Connectivity Graph');

% run the simulation on nanosensor
[t_nano, ~, ~, ~, ~, py2_nano] = simulate_activity(ny1, ny2, ne, Kon, Koff, Kcat, valency, particle_diameter, time_step, t_max);

% run the simulation as free peptide
valency = 1;
[t_free, ~, ~, ~, ~, py2_free] = simulate_activity(ny1, ny2, ne, Kon, Koff, Kcat, valency, particle_diameter, time_step, t_max);

% plot the results
figure; plot(t_nano, py2_nano{1});
hold on; plot(t_free, py2_free{1});
title('YES^{2} on Nanosensor vs. Free Peptide');
legend('Nanosensor', 'Free Peptide');

% % run the simulation on nanosensor
% valency = 40;
% [t_nano, ~, py1_nano, ~, ~, ~] = simulate_activity(ny2, ny1, ne, Kon, Koff, Kcat, valency, particle_diameter, time_step, t_max);
% % run the simulation as free peptide
% valency = 1;
% [t_free, ~, py1_free, ~, ~, ~] = simulate_activity(ny2, ny1, ne, Kon, Koff, Kcat, valency, particle_diameter, time_step, t_max);
% % plot the results
% figure; plot(t_nano, py1_nano{1});
% hold on; plot(t_free, py1_free{1});
% title('YES on Nanosensor vs. Free Peptide');
% legend('Nanosensor', 'Free Peptide');
