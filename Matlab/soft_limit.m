function result = soft_limit(x,range)
result = range-log(1+exp((range-x)*10))/10;
end