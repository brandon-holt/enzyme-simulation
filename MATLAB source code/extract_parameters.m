function [num_y1, num_y2, num_enzymes, kon, koff, kcat, enzyme_names, substrate_names] = extract_parameters(excel_file)

    [num, txt, ~] = xlsread(excel_file, 1);
    substrate_names = txt(2:length(txt))';
    ny = zeros(2, length(num));
    for i = 1:length(num)
        ny(num(i,1),i) = num(i,2);
    end
    [number_per_enzyme, txt, ~] = xlsread(excel_file, 2);
    enzyme_names = txt(2:length(txt))';
    kon = zeros(numel(substrate_names), numel(enzyme_names), 2);
    koff = zeros(numel(substrate_names), numel(enzyme_names), 2);
    kcat = zeros(numel(substrate_names), numel(enzyme_names), 2);
    [~, txt, ~] = xlsread(excel_file, 3);
    [kon, koff, kcat] = read_kinetic_params(kon, koff, kcat, txt, 1);
    [~, txt, ~] = xlsread(excel_file, 4);
    [kon, koff, kcat] = read_kinetic_params(kon, koff, kcat, txt, 2);
    
    num_y1 = ny(1,:); % number of YES substrates
    num_y2 = ny(2,:); % number of YES2 substrates
    num_enzymes = number_per_enzyme';
    
end

%% Function to Read Kinetic Parameters from the Excel txt
function [kon, koff, kcat] = read_kinetic_params(kon, koff, kcat, txt, side)

    % side = 1 or 2 (left or right of YES2)
    for r = 2:length(txt)
        for c = 2:width(txt)
            k = strsplit(txt{r,c},',');
            k = cellfun(@(c) str2double(c), k);
            kon(r-1,c-1,side) = k(1);
            koff(r-1,c-1,side) = k(2);
            kcat(r-1,c-1,side) = k(3);
        end
    end
end