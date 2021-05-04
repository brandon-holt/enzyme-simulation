% generate histograms of initial velocity for YES or YES2 gate
clear;clc;close all;

% values from Anirudh
Etot = 1e-3; % uM
S0 = 1e-2; % uM
kCat = 1.908e-3; % s^-1
kM = 0.175; %uM
noise_kinetic_factor = 2; % kM*2 and kcat/2

% conversion constants
uM_to_num = 1e3; % volume, constant to convert uM to number of molecules
kon_koff_ratio = 10; % kon = kon_koff_ratio * koff
time_units = 60; % seconds to mins

% converted inputs
ny = round(S0 * uM_to_num);
num_enzymes = round(Etot * uM_to_num);
kcat = time_units * kCat;
km = kM * uM_to_num; % convert to number
kon = time_units * kon_koff_ratio * kcat / (km - kon_koff_ratio);
koff = time_units * kcat / (km - kon_koff_ratio);

% other inputs
prob_jump = 0.2;
time_step = 0.1;
tMax = 100; % minutes

% number of runs
n = 1000;
vel_yes = zeros(n,1);
vel_yes2 = zeros(n,1);

for i = 1:n
    
    % simulate the YES gate
    [~, ~, py1, ~, ~, ~] = simulate_activity(ny, 0, num_enzymes, kon, koff, kcat, prob_jump, time_step, tMax);
    
    % simulate the YES2 gate
    [~, ~, ~, ~, ~, py2] = simulate_activity(0, ny, num_enzymes, kon, koff, kcat, prob_jump, time_step, tMax);
    
    % store the values
    vel_yes(i) = py1{1}(end) / tMax;
    vel_yes2(i) = py2{1}(end) / tMax;
    
end

% plot the results
figure; histogram(vel_yes); hold on; histogram(vel_yes2); legend('YES gate', 'YES^{2} gate');
title('Distribution of Initial Velocities for Single Enzyme'); xlabel('Velocity (copies/min)'); ylabel('Frequency');


