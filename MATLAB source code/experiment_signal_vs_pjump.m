% experiment testing YES vs. YES2 at various protease concentrations
clear; clc;

save_results = true;

% parameters
num_substrates = 1e3;
num_enzymes = 1e2;
kon = 3;
koff = 1;
kcat = 3;
time_step = 0.1; t_max = 50;

% signal only, YES2
num_trials = 10;
pjump_range = linspace(0,1,num_trials);
signal_data = cell(num_trials, 1);
num_y1 = 0;
num_y2 = num_substrates;
for i = 1:num_trials
    prob_jump = pjump_range(i);
    [t, ~, ~, ~, ~, p] = simulate_activity(num_y1, num_y2, num_enzymes, kon, koff, kcat, prob_jump, {'Signal'}, {'YES2 Gate'}, time_step, t_max);
    signal_data{i} = [t, p{1}];
end

% plot results
fig = figure;
for i = 1:num_trials
    subplot(1,2,1);
    label = strcat('p_{jump} = ', num2str(pjump_range(i)));
    plot(signal_data{i}(:,1), signal_data{i}(:,2), 'DisplayName', label); hold on;
end
legend('Location', 'northwest'); title('Effect of p_{jump}');
ylabel('Products (number)'); xlabel('Time (seconds)');

subplot(1,2,2);
hill_coeffs = cellfun(@(c) hill(c(:,1), c(:,2)), signal_data);
plot(pjump_range, hill_coeffs); title('Effect of p_{jump} on Hill');
ylabel('Hill Coefficient'); xlabel('p_{jump}');

% save results
if save_results
    filename = strrep(strcat('/figures/run_', datestr(now), '.fig'),':','_');
    filename = strrep(filename, '-', '_');
    filename = strrep(filename, ' ', '_');
    savefig(fig, fullfile(pwd, filename));
end