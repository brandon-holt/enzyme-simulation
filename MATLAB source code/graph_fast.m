function [] = graph_fast(t, py1, py2, s_names)
    
    figure; 
    group_size = 50;
    indicies = 1:group_size:numel(t);
    
    max_substrates = max(numel(py1), numel(py2));
    al = cell(max_substrates, 2);
    
    for si = 1:max_substrates
        al{si, 1} = animatedline('Color', rand(1,3), 'DisplayName', strcat(s_names{si}, ' (YES) '));
        al{si, 2} = animatedline('Color', rand(1,3), 'DisplayName', strcat(s_names{si}, ' (YES^{2}) '));
    end
    
    for i = 1:numel(indicies)-1
        
        ind = indicies(i):indicies(i+1);
        
        for si = 1:max_substrates
        
            title('Substrate Gate Signal');
            
            if ~isempty(py1{si})
                addpoints(al{si, 1}, t(ind), py1{si}(ind)); hold on;
            end
            
            if ~isempty(py2{si})
                addpoints(al{si, 2}, t(ind), py2{si}(ind)); hold on;
            end
            
            xlabel('Time (seconds)'); ylabel('Products (number)');
            legend('Location', 'northwest'); drawnow;
        
        end
        
    end
    
    % delete lines with no data
    for i = 1:max_substrates
        if sum(py1{i}) == 0
            delete(al{i,1});
        end
        if sum(py2{i}) == 0
            delete(al{i,2});
        end
    end

end