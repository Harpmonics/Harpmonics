l = 1;
c = 2;
d = 1;
theta = deg2rad(45);

xl1 = -1;
xl2 = 1;

val_fun = @(x0,y0) soft_limit(integral(@(x) ((x-x0).^2+y0^2).^(-c/2), xl1, xl2), 10);

pixel_fun = @(x1,x2) integral(@(x) val_fun(x*sin(theta), sqrt((x*cos(theta))^2+d^2)), x1, x2);
