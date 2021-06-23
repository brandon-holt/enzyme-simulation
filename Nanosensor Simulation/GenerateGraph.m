function [G, graph_matrix] = GenerateGraph(num_nodes, num_edges_per_node)
    
    % generates a graph where each node has num_edges_per_node
    % N is size num_nodes x num_edges_per_node
    
    G = graph;
    G = addnode(G, num_nodes);
    
    graph_matrix = zeros(num_nodes, num_edges_per_node);
    
    for col = 1:num_edges_per_node
        
        for row = 1:num_nodes
            
            if graph_matrix(row, col) ~= 0; continue; end % if we already have this value, continue

            for value = 1:num_nodes

                if value == row; continue; end % if value is same as row, continue
                
                if sum(graph_matrix(row,:) == value) > 0; continue; end % if this row contains connection to this value already, continue
                
                if sum(graph_matrix(value,:) == 0) == 0; continue; end % move on if this row is complete
                
                first_free_col = find(graph_matrix(row,:) == 0, 1, 'first');
                graph_matrix(row, first_free_col) = value;
                
                first_free_col_other = find(graph_matrix(value,:) == 0, 1, 'first');
                graph_matrix(value, first_free_col_other) = row;
                
                G = addedge(G, row, value);
                
                break; % once we found a value for this spot, break and move onto next row
                
            end

        end
        
    end
    
    plot(G);
    
    accuracy = sum(degree(G) == num_edges_per_node) / num_nodes
    
end