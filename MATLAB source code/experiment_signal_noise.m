% expriment testing signal+noise vs. noise only for both YES and YES2 gates
clear; clc;

save_results = true;

% parameters
num_substrates = 1e3;
num_signal_enzymes = 1e2;
num_noise_enzymes = .5e2;
kon = zeros(1,2,2);
kon(:,:,1) = [3, 2.9]; kon(:,:,2) = kon(:,:,1); 
koff = zeros(1,2,2);
koff(:,:,1) = [1, 0.9]; koff(:,:,2) = koff(:,:,1); 
kcat = zeros(1,2,2);
kcat(:,:,1) = [1, 0.9]; kcat(:,:,2) = kcat(:,:,1); 
time_step = 0.1; t_max = 20;
prob_jump = 0.2;

% noise only, YES
num_y1 = num_substrates;
num_y2 = 0;
num_enzymes = [0, num_noise_enzymes];
[t, ~, p, ~, ~, ~] = simulate_activity(num_y1, num_y2, num_enzymes, kon, koff, kcat, prob_jump, {'Noise'}, {'YES Gate'}, time_step, t_max);
noise_yes = [t, p{1}];

% noise + signal, YES
num_y1 = num_substrates;
num_y2 = 0;
num_enzymes = [num_signal_enzymes, num_noise_enzymes];
[t, ~, p, ~, ~, ~] = simulate_activity(num_y1, num_y2, num_enzymes, kon, koff, kcat, prob_jump, {'Signal + Noise'}, {'YES Gate'}, time_step, t_max);
noise_signal_yes = [t, p{1}];

% noise + signal, YES^2
num_y1 = 0;
num_y2 = num_substrates;
num_enzymes = [0, num_noise_enzymes];
[t, ~, ~, ~, ~, p] = simulate_activity(num_y1, num_y2, num_enzymes, kon, koff, kcat, prob_jump, {'Noise'}, {'YES Gate'}, time_step, t_max);
noise_yes2 = [t, p{1}];

% noise + signal, YES^2
num_y1 = 0;
num_y2 = num_substrates;
num_enzymes = [num_signal_enzymes, num_noise_enzymes];
[t, ~, ~, ~, ~, p] = simulate_activity(num_y1, num_y2, num_enzymes, kon, koff, kcat, prob_jump, {'Signal + Noise'}, {'YES Gate'}, time_step, t_max);
noise_signal_yes2 = [t, p{1}];

% plot results
fig = figure;
pct_noise = num2str(round(100 * num_noise_enzymes / num_signal_enzymes));
subplot(1,2,1); plot(noise_yes(:,1), noise_yes(:,2)); hold on; plot(noise_signal_yes(:,1), noise_signal_yes(:,2));
title(strcat('YES Gate: Signal vs. Noise (', pct_noise, '%)')); ylabel('Products (number)'); xlabel('Time (seconds)');
legend('Noise Only', 'Signal + Noise');
subplot(1,2,2); plot(noise_yes2(:,1), noise_yes2(:,2)); hold on; plot(noise_signal_yes2(:,1), noise_signal_yes2(:,2));
title(strcat('YES Gate: Signal vs. Noise (', pct_noise, '%)')); ylabel('Products (number)'); xlabel('Time (seconds)');
legend('Noise Only', 'Signal + Noise');

% save results
if save_results
    filename = strrep(strcat('/figures/run_', datestr(now), '.fig'),':','_');
    filename = strrep(filename, '-', '_');
    filename = strrep(filename, ' ', '_');
    savefig(fig, fullfile(pwd, filename));
end