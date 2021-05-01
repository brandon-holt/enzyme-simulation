% run this to generate panels C and D
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

% format kinetic constants for two enzymes
kM = kM * ones(1,2,2);
kCat = kCat * ones(1,2,2);
kM(1,2,:) = noise_kinetic_factor * kM(1,2,:);
kCat(1,2,:) = kCat(1,2,:) / noise_kinetic_factor;

% converted inputs
ny = round(S0 * uM_to_num);
num_enzymes = round(Etot * uM_to_num);
kcat = time_units * kCat;
km = kM * uM_to_num; % convert to number
kon = time_units .* kon_koff_ratio .* kcat ./ (km - kon_koff_ratio);
koff = time_units .* kcat ./ (km - kon_koff_ratio);

% other inputs
prob_jump = 0.2;
time_step = 0.1;
tMax = 300; % minutes

% simulate the YES gate NOISE only
[ty_noise, ~, py_noise, ~, ~, ~] = simulate_activity(ny, 0, [0, num_enzymes], kon, koff, kcat, prob_jump, time_step, tMax);
% simulate the YES gate SIGNAL + NOISE
[ty_sig_noise, ~, py_sig_noise, ~, ~, ~] = simulate_activity(ny, 0, [num_enzymes, num_enzymes], kon, koff, kcat, prob_jump, time_step, tMax);
% plot results
figure; subplot(1,2,1); plot(ty_noise, py_noise{1}); hold on; plot(ty_sig_noise, py_sig_noise{1});
title('Panel C'); legend('Off-target only', 'On-target + Off-target'); xlabel('Time (min)'); ylabel('Signal (no. products)');

% simulate the YES2 gate NOISE only
[ty2_noise, ~, ~, ~, ~, py2_noise] = simulate_activity(0, ny, [0, num_enzymes], kon, koff, kcat, prob_jump, time_step, tMax);
% simulate the YES2 gate SIGNAL + NOISE
[ty2_sig_noise, ~, ~, ~, ~, py2_sig_noise] = simulate_activity(0, ny, [num_enzymes, num_enzymes], kon, koff, kcat, prob_jump, time_step, tMax);
% plot results
subplot(1,2,2); plot(ty2_noise, py2_noise{1}); hold on; plot(ty2_sig_noise, py2_sig_noise{1});
title('Panel D'); legend('Off-target only', 'On-target + Off-target'); xlabel('Time (min)'); ylabel('Signal (no. products)');

% plot difference between sig+noise - noise as classification power
class_power_yes = (py_sig_noise{1} - py_noise{1}) / ny;
class_power_yes2 = (py2_sig_noise{1} - py2_noise{1}) / ny;
figure; plot(ty_noise, class_power_yes); hold on; plot(ty2_noise, class_power_yes2);
title('Panel E'); legend('YES', 'YES^{2}'); xlabel('Time (min)'); ylabel('Classification Power');