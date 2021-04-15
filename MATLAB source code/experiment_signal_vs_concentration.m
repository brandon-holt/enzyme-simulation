% experiment testing YES vs. YES2 at various protease concentrations
clear; clc;

save_results = true;

% parameters
num_substrates = 1e3;
num_signal_enzymes = [1, 5 * round(logspace(0, 4, 30))];
kon = 3;
koff = 1;
kcat = 3;
time_step = 0.1; t_max = 5;
prob_jump = 0.2;

% signal only, YES
yes_data = [];
num_y1 = num_substrates;
num_y2 = 0;
for num_enzymes = num_signal_enzymes
    [t, ~, p, ~, ~, ~] = simulate_activity(num_y1, num_y2, num_enzymes, kon, koff, kcat, prob_jump, {'Signal'}, {'YES Gate'}, time_step, t_max);
    velocity = (p{1}(end) - p{1}(1)) / t_max; % copies / s
    yes_data = [yes_data, velocity];
end

% signal only, YES2
yes2_data = [];
num_y1 = 0;
num_y2 = num_substrates;
for num_enzymes = num_signal_enzymes
    [t, ~, ~, ~, ~, p] = simulate_activity(num_y1, num_y2, num_enzymes, kon, koff, kcat, prob_jump, {'Signal'}, {'YES2 Gate'}, time_step, t_max);
    velocity = (p{1}(end) - p{1}(1)) / t_max; % copies / s
    yes2_data = [yes2_data, velocity];
end

% calculate Hill Coefficient
hill_yes = hill(num_signal_enzymes, yes_data);
hill_yes2 = hill(num_signal_enzymes, yes2_data);

% plot results
fig = figure;
plot(num_signal_enzymes, yes_data, '-o'); set(gca, 'XScale', 'log');
hold on; plot(num_signal_enzymes, yes2_data, '-x'); set(gca, 'XScale', 'log');
leg1 = strcat('YES: Hill = ', num2str(hill_yes));
leg2 = strcat('YES^{2}: Hill = ', num2str(hill_yes2));
legend(leg1, leg2, 'Location', 'northwest');
title('Signal Dose Dependence'); ylabel('Velocity (copies / s)'); xlabel('Enzymes (number)');

% save results
if save_results
    filename = strrep(strcat('/figures/run_', datestr(now), '.fig'),':','_');
    filename = strrep(filename, '-', '_');
    filename = strrep(filename, ' ', '_');
    savefig(fig, fullfile(pwd, filename));
end
