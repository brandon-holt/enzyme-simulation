% experiment testing YES vs. YES2 at various protease concentrations
clear; clc;

save_results = true;

% parameters
num_substrates = 1e3;
num_signal_enzymes = [1, 5 * round(logspace(0, 4, 30))];
kon = 3;
koff = 3;
kcat = 3;
time_step = 0.1;
t_max = 30;

num_trials = 10;
prob_jump = 0.2;
pjump_vec = linspace(0,1,num_trials);
hill_vec = zeros(1, num_trials);

s_data = cell(num_trials, 1);
for i = 1:num_trials
    
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
    hill_vec(i) = hill(num_signal_enzymes, yes2_data);
    
    % save dosing data
    s_data{i} = yes2_data;
    
end


% plot results
fig = figure;

for i = 1:num_trials
    
    subplot(1,2,1);
    plot(num_signal_enzymes, s_data{i}, '-x', 'DisplayName', strcat('p_{jump} = ', num2str(pjump_vec(i))));
    set(gca, 'XScale', 'log'); hold on; legend('Location', 'northwest');
    title('Signal Dose Dependence'); ylabel('Velocity (copies / s)'); xlabel('Enzymes (number)');

end

subplot(1,2,2);
plot(pjump_vec, hill_vec); title('Effect of P_{jump} on Hill');
xlabel('p_{jump}'); ylabel('Hill Coefficient');

% save results
if save_results
    filename = strrep(strcat('/figures/run_', datestr(now), '.fig'),':','_');
    filename = strrep(filename, '-', '_');
    filename = strrep(filename, ' ', '_');
    savefig(fig, fullfile(pwd, filename));
end