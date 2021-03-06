% first get a products over time graph of an enzyme with two uneven
% substrates (one high kcat the other very low)
clear; clc;

Km = 2.9132e-5 * (6e23);
Kcat = 4.9286e-4 * (6e23);
Kon = Kcat / (Km - 1);
Koff = Kon;

factor = 100;
kon = zeros(1,1,2); kon(1,1,1) = factor * Kon; kon(1,1,2) = Kon / factor;
koff = zeros(1,1,2); koff(1,1,1) = factor * Koff; koff(1,1,2) = Koff / factor;
kcat = zeros(1,1,2); kcat(1,1,1) = factor * Kcat; kcat(1,1,2) = Kcat / factor;

save_results = true;

% parameters
num_substrates = 2e3;
num_signal_enzymes = [1, 5 * round(logspace(0, 6.3, 10))];
time_step = .1;
prob_jump = 0;

% signal only, YES2 early time
t_max = 5;
yes2_dataE = [];
num_y1 = 0;
num_y2 = num_substrates;
for num_enzymes = num_signal_enzymes
    [t, ~, ~, ~, ~, p] = simulate_activity(num_y1, num_y2, num_enzymes, kon, koff, kcat, prob_jump, {'Signal'}, {'YES2 Gate'}, time_step, t_max);
    velocity = (p{1}(end) - p{1}(1)) / 1; % copies / s
    yes2_dataE = [yes2_dataE, velocity];
end

% signal only, YES2 later time
t_max = 50;
yes2_data = [];
num_y1 = 0;
num_y2 = num_substrates;
for num_enzymes = num_signal_enzymes
    [t, ~, ~, ~, ~, p] = simulate_activity(num_y1, num_y2, num_enzymes, kon, koff, kcat, prob_jump, {'Signal'}, {'YES2 Gate'}, time_step, t_max);
    velocity = (p{1}(end) - p{1}(1)) / 1; % copies / s
    yes2_data = [yes2_data, velocity];
end

% calculate Hill Coefficient
hill_yes2E = hill(num_signal_enzymes, yes2_dataE);
hill_yes2 = hill(num_signal_enzymes, yes2_data);

% plot results
fig = figure;
plot(num_signal_enzymes, yes2_dataE, '-x'); hold on;
plot(num_signal_enzymes, yes2_data, '-x'); set(gca, 'XScale', 'log');
leg2E = strcat('YES^{2} Early: Hill = ', num2str(hill_yes2E));
leg2 = strcat('YES^{2} Later: Hill = ', num2str(hill_yes2));
legend(leg2E, leg2, 'Location', 'northwest');
title('Signal Dose Dependence'); ylabel('Velocity (copies / s)'); xlabel('Enzymes (number)');

% save results
if save_results
    filename = strrep(strcat('/figures/run_', datestr(now), '.fig'),':','_');
    filename = strrep(filename, '-', '_');
    filename = strrep(filename, ' ', '_');
    savefig(fig, fullfile(pwd, filename));
end