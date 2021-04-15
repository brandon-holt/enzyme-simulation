function [h] = hill(x, y)
    norm_resp = y / max(y);
    ec90 = ec(x, norm_resp, 90);
    ec10 = ec(x, norm_resp, 5);
    h = log10(81) / log10(ec90 / ec10);
end

function [ec] = ec(x, norm_resp, i)
    i = i / 100;
    i_hi = find(norm_resp>i, 1, 'first'); i_lo = i_hi - 1;
    if i_lo == 0; ec = x(1); return; end
    pct_btw = (i - norm_resp(i_lo)) / (norm_resp(i_hi) - norm_resp(i_lo));
    ec = x(i_lo) + (pct_btw * (x(i_hi) - x(i_lo)));
end