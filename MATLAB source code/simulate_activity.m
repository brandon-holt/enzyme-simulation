function [t, sy1, py1, sy2, iy2, py2] = simulate_activity(ny1, ny2, num_enzymes, kon, koff, kcat, prob_jump, enzyme_names, substrate_names, time_step, tMax)

    save_results = true;
    
    % global parameters, substrates on rows, enzymes on columns
    global Kon; global Koff; global Kcat; global timeStep;
    global ne; global p_jump; global YES2; global enzymes; 
    if numel(kon) == 1; Kon = zeros(1,1,2); Kon(1,1,:) = kon;
    else; Kon = kon; end
    if numel(koff) == 1; Koff = zeros(1,1,2); Koff(1,1,:) = koff;
    else; Koff = kon; end
    if numel(kcat) == 1; Kcat = zeros(1,1,2); Kcat(1,1,:) = kcat;
    else; Kcat = kon; end
    timeStep = time_step; % s, resolution
    ne = num_enzymes; % number of enzymes
    p_jump = prob_jump; % probability of an enzyme jumping to YES2 neighbor after cleavage

    % initialize vectors
    enzymes = cell(numel(ne), 1);
    for e = 1:numel(ne)
        enzymes{e} = zeros(ne(e), 1); % 0 = free, 1 = bound
    end
    YES1 = cell(numel(ny1), 1);
    for y1 = 1:numel(ny1)
        YES1{y1} = zeros(ny1(y1), 1, 3); % 0 = free, 1:numel(ne) = bound to enzyme, numel(ne)+1 = cleaved
        YES1{y1}(:,:,2:3) = NaN; % 2 = goal state, 3 = time left
    end
    YES2 = cell(numel(ny2), 1);
    for y2 = 1:numel(ny2)
        YES2{y2} = zeros(ny2(y2), 2, 3); % 0 = free, 1:numel(ne) = bound to enzyme, numel(ne)+1 = cleaved
        YES2{y2}(:,:,2:3) = NaN; % 2 = goal state, 3 = time left
    end
    max_copies = max([ny1, ny2]);
    max_substrates = max(numel(ny1), numel(ny2));
    sy1 = cell(numel(ny1), 1); % substrates of YES gate
    py1 = cell(numel(ny1), 1); % products of YES gate
    sy2 = cell(numel(ny2), 1); % substrates of YES2 gate
    iy2 = cell(numel(ny2), 1); % intermediates of YES2 gate
    py2 = cell(numel(ny2), 1); % products of YES2 gate

    %% Main Simulation
    currentTime = 0; % s
    fprintf('simulation progress: %3d%%',0)
    while currentTime < tMax

        % record data, check for stop condition, and display progress
        finish = false;
        max_progress = 0;
        for sub_ind = 1:max_substrates
            % record time and products, just first substrate of each for now
            sy1{sub_ind} = [sy1{sub_ind}; sum(YES1{sub_ind}(:,1,1) == 0)];
            py1{sub_ind} = [py1{sub_ind}; sum(YES1{sub_ind}(:,1,1) == numel(ne)+1)];
            sy2{sub_ind} = [sy2{sub_ind}; sum(sum(YES2{sub_ind}(:,:,1), 2) == 0)];
            py2{sub_ind} = [py2{sub_ind}; sum(prod(YES2{sub_ind}(:,:,1) == numel(ne)+1, 2))];
            iy2{sub_ind} = [iy2{sub_ind}; ny2(sub_ind) - sy2{sub_ind}(end) - py2{sub_ind}(end)];

            % display current progress (+ 1 to avoid Inf issue with 0 substrates)
            current_progress = max([currentTime/tMax, py1{sub_ind}(end)/(ny1(sub_ind)+1), py2{sub_ind}(end)/(ny2(sub_ind)+1)]);
            max_progress = max(max_progress, current_progress);

            % stop the simulation if all substrates are cleaved
            seconds_post_ss = 5;
            last_i = round(seconds_post_ss / timeStep);
            if last_i >= numel(py1{sub_ind}); continue; end
            if all(py1{sub_ind}(end-last_i:end) == ny1(sub_ind)) && all(py2{sub_ind}(end-last_i:end) == ny2(sub_ind)); finish = true; end
        end
        fprintf('\b\b\b\b%3d%%',floor(max_progress*100))
        if finish; break; end

        % process each substrate vector in random order because orders matters for YES2 gate
        perm0 = randperm(max_copies); perm1 = randperm(max_copies); perm2 = randperm(max_copies);
        for ci = 1:max_copies
            for si = 1:max_substrates

                ci0 = perm0(ci);
                if si <= numel(YES1)
                    if ci0 <= length(YES1{si})
                        YES1{si}(ci0,1,:) = Process(YES1{si}(ci0,1,:), si, ci0, 0);
                    end
                end

                ci1 = perm1(ci);
                if si <= numel(YES2)
                    if ci1 <= length(YES2{si})
                        YES2{si}(ci1,1,:) = Process(YES2{si}(ci1,1,:), si, ci1, 1);
                    end
                end

                ci2 = perm2(ci);
                if si <= numel(YES2)
                    if ci2 <= length(YES2{si})
                        YES2{si}(ci2,2,:) = Process(YES2{si}(ci2,2,:), si, ci2, 2);
                    end
                end

            end
        end

        % update time
        currentTime = currentTime + timeStep;

    end

    fprintf('\b\b\b\b%3d%%\n',100)

    t = 0:timeStep:tMax;
    t = t(1:numel(py1{1}))';

    if save_results
        filename = strrep(strcat('/results/run_', datestr(now), '.mat'),':','_');
        filename = strrep(filename, '-', '_'); filename = strrep(filename, ' ', '_');
        save(fullfile(pwd, filename), 't', 'sy1', 'py1', 'sy2', 'iy2', 'py2', 'Kcat', 'Koff', 'Kon', 'ne', 'ny1', 'ny2', 'p_jump', 'substrate_names', 'enzyme_names');
    end
    
end

%% Function to process substrate vector
function vec = Process(vec, si, ci, y2i)
    
    % si = substrate index, ci = copy index, y2i = YES2 index (left or right)

    % what each index means in the 3rd dimension
    state = 1; task = 2; time = 3;
    
    % declare global variables from main thread
    global Kon; global Koff; global Kcat; global timeStep;
    global YES2; global enzymes; global p_jump; global ne;
    
    % process possible events on this substrate
    if ~isnan(vec(1,1,task)) % if an action is in progress

        if vec(1,1,time) - timeStep <= 0 % if the task completes during this timestep
            % free the enzyme if cleavage or binding event
            ei = vec(1,1,state); % enzyme index
            if vec(1,1,task) == 0 || vec(1,1,task) == numel(ne)+1
                boundEnzyme = find(enzymes{ei}==1, 1, 'first'); % find bound enzyme
                enzymes{ei}(boundEnzyme) = 0; % unbind enzyme
                % if we are in a YES2 AND the enzyme jumps AND other substrate is unbound AND other substrate has no task in progress
                if y2i > 0 && rand < p_jump && YES2{si}(ci,3-y2i,state) == 0 && isnan(YES2{si}(ci,3-y2i,task))
                    enzymes{ei}(boundEnzyme) = 1; % bind enzyme
                    YES2{si}(ci,3-y2i,task) = ei; % task = binding by enzyme index
                    YES2{si}(ci,3-y2i,time) = exprnd(1/Kon(si, ei, y2i)); % time to bind
                end
            end
            % state becomes the task, reset task and time
            vec(1,1,state) = vec(1,1,task);
            vec(1,1,[task, time]) = NaN;
        else % the task does not complete during this timestep
            vec(1,1,time) = vec(1,1,time) - timeStep;
        end

    elseif vec(1,1,state) == 0 % if the substrate is free of enzyme

        % encounter enzyme weighted by probability based on abundance
        free_enzymes = cellfun(@(enz_arr) sum(enz_arr==0), enzymes);
        cum_dist = cumsum(ne / sum(ne));
        rand_e = find(rand < cum_dist, 1, 'first'); % index of randomly encountered enzyme
        if rand < free_enzymes(rand_e)/ne(rand_e) % probability of encountering one of the free enzymes in this group
            freeEnzyme = find(enzymes{rand_e}==0, 1, 'first'); % index of free enzyme
            if y2i == 0; y2i = 1; end % for indexing Kon properly
            enzymes{rand_e}(freeEnzyme) = 1; % bind enzyme 
            vec(1,1,task) = rand_e; % begin binding by index
            vec(1,1,time) = exprnd(1/Kon(si, rand_e, y2i));
        end

    elseif vec(1,1,state) ~= numel(ne)+1 % if the substrate is bound to an enzyme
        
        if y2i == 0; y2i = 1; end % for indexing Kcat, Koff properly
        ei = vec(1,1,state); % enzyme index
        t_to_finish_cutting = exprnd(1/Kcat(si, ei, y2i));
        t_to_unbind = exprnd(1/Koff(si, ei, y2i));
        if (t_to_unbind < t_to_finish_cutting) % if time to unbind is faster
            vec(1,1,task) = 0; % unbinding in progress
            vec(1,1,time) = t_to_unbind;
        else % search through all available enzymes
            vec(1,1,task) = numel(ne)+1; % cutting in progress
            vec(1,1,time) = t_to_finish_cutting;
        end

    end
    
end