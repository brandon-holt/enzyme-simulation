function [graph_matrix] = RegularGraph(n, k)
    
    graph_matrix = zeros(n, k);
    
    for col = 1:k
        
        for row = 1:n
            
            if graph_matrix(row, col) ~= 0; continue; end % if we already have this value, continue

            for value = 1:n

                if value == row; continue; end % if value is same as row, continue
                
                if sum(graph_matrix(row,:) == value) > 0; continue; end % if this row contains connection to this value already, continue
                
                if sum(graph_matrix(value,:) == 0) == 0; continue; end % move on if this row is complete
                
                first_free_col = find(graph_matrix(row,:) == 0, 1, 'first');
                graph_matrix(row, first_free_col) = value;
                
                first_free_col_other = find(graph_matrix(value,:) == 0, 1, 'first');
                graph_matrix(value, first_free_col_other) = row;
                
                break; % once we found a value for this spot, break and move onto next row
                
            end

        end
        
    end
    
    
end