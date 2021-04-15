clear; clc; close all;

% initialize parameters
time_step = 0.1; % s, resolution
t_max = 400; % s, total time simulated
prob_jump = 0; % probability of an enzyme jumping to YES2 neighbor after cleavage

% pull parameters from excel doc
[ny1, ny2, ne, Kon, Koff, Kcat, e_names, s_names] = extract_parameters('parameters.xlsx');

% run the simulation
[t, sy1, py1, sy2, iy2, py2] = simulate_activity(ny1, ny2, ne, Kon, Koff, Kcat, prob_jump, e_names, s_names, time_step, t_max);

% plot the results
graph_fast(t, py1, py2, s_names);

% plot the heatmap of activity: Km = Koff + Kcat / Kon
figure; kcatkm = Kcat .* Kon ./ (Koff + Kcat);
subplot(1,2,1); heatmap(kcatkm(:,:,1), 'XData', e_names, 'YData', s_names);
title('Catalytic Coefficient A (k_{cat}/K_{M})'); xlabel('Enzymes'); ylabel('Substrates');
subplot(1,2,2); heatmap(kcatkm(:,:,2), 'XData', e_names, 'YData', s_names);
title('Catalytic Coefficient B (k_{cat}/K_{M})'); xlabel('Enzymes'); ylabel('Substrates');

% plot the protease expression bar graph
figure; bar(categorical(e_names), ne);
ylabel('Number of Enzymes'); title('Enzyme Abundance');

% plot examples YES2
yes2i = find(ny2 > 0, 1, 'first');
if ~isempty(yes2i)
    figure; plot(t, sy2{yes2i});  hold on; plot(t, py2{yes2i}); hold on; plot(t, iy2{yes2i}/2);
    title('Populations over Time'); xlabel('Time (seconds)'); ylabel('Number');
    legend('Substrates', 'Products', 'Intermediates');
end