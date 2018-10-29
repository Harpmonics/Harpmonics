l = 1;
c = 2;

dim = [49,49];
res = 0.1;
center = [l/2,0];

val = zeros(dim);
val_fun = @(x0,y0) soft_limit(integral(@(x) ((x-x0).^2+y0^2).^(-c/2), 0, l), 10);

for i = 1:dim(1)
    for j = 1:dim(2)
        pos = ([i,j]-dim/2)*res+center;
        x0 = pos(1);
        y0 = pos(2);
        val(i,j) = val_fun(x0,y0);
    end
end

surf(val);
