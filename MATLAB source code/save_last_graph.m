function save_last_graph()
    filename = strrep(strcat('/figures/run_', datestr(now), '.fig'),':','_');
    filename = strrep(filename, '-', '_');
    filename = strrep(filename, ' ', '_');
    savefig(fullfile(pwd, filename));
end