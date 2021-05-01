% run this to generate the data for panels A and B
clear;clc;close all;

% values from Anirudh
Etot = 0.1; % uM
S0 = 1; % uM
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
tMax = 300; % minutes

% simulate the YES gate
[t1, sy1, py1, ~, ~, ~] = simulate_activity(ny, 0, num_enzymes, kon, koff, kcat, prob_jump, time_step, tMax);

% plot results
figure; subplot(1,2,1); plot(t1, sy1{1}); hold on; plot(t1, py1{1});
title('Panel A'); xlabel('Time (min)'); ylabel('Signal (no. products)'); legend('S', 'P');

% simulate the YES2 gate
[t2, ~, ~, sy2, iy2, py2] = simulate_activity(0, ny, num_enzymes, kon, koff, kcat, prob_jump, time_step, tMax);

% plot results
iy2{1} = iy2{1} / 2;
subplot(1,2,2); plot(t2, sy2{1}); hold on; plot(t2, iy2{1}); hold on; plot(t2, py2{1});
title('Panel B'); xlabel('Time (min)'); ylabel('Signal (no. products)'); legend('S','I','P');