
data = [];

for n = 2:100
    
    for k = 1:n-1
        
        tic;
        RegularGraph(n, k);
        runtime = toc;
        
        data = [data; n, k, runtime];
        
    end
    
end

close all;
figure; scatter(data(:,1), data(:,3));
figure; scatter(data(:,2), data(:,3));