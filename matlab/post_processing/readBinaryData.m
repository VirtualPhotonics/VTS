function data = readBinaryData(filename, size)

fid = fopen(filename,'rb');
data = fread(fid,size,'double');
fclose(fid);