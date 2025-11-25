# This is an example of python code to convert Zemax Output to a RayIlluminationDatabase
# for subsequent reading into MCCL
# This site provided helpful example code: 
# https://community.zemax.com/code-exchange-10/python-reading-writing-binary-files-zrd-zbf-dat-sdf-3116
import sys
import struct
import numpy as np
# check if enough arguments are provided
if len(sys.argv) < 3:
   print("Usage: python3 zemax_zrd_output_to_mccl_source.py <fileToConvert> <convertedFile or 'debug'>")
   sys.exit(0)
else:
   input_filename = sys.argv[1]
   output_filename = sys.argv[2]

# 0ne ray = 208 bytes 
# Open Zemax file
try:
   with open(input_filename,'rb') as f:
      # read header
      version = int.from_bytes(f.read(4),byteorder='little')
      max_seg = int.from_bytes(f.read(4),byteorder='little')
      print('Zemax ZRD version = ' + str(version))
      print('Zemax max seg = ' + str(max_seg))
      loops = 0
      X = []      
      Y = []      
      Z = []      
      Ux = []      
      Uy = []      
      Uz = []      
      Intensity = []      
      while True:
         # number of segments for each ray: only need last
         segs = int.from_bytes(f.read(4),byteorder='little')
         if segs == 0:
            break
         for i in range(0,segs,1):
             dum1 = int.from_bytes(f.read(40)) # 10 ints: not needed
             dum2 = struct.unpack('d',f.read(8))[0] # 1 double: not needed
             dum3 = struct.unpack('d',f.read(8))[0] # 1 double: not needed
             dumX = struct.unpack('d',f.read(8))[0]
             dumY = struct.unpack('d',f.read(8))[0]
             dumZ = struct.unpack('d',f.read(8))[0]
             dumUx = struct.unpack('d',f.read(8))[0]
             dumUy = struct.unpack('d',f.read(8))[0]
             dumUz = struct.unpack('d',f.read(8))[0]
             dum4 = struct.unpack('d',f.read(8))[0] # 1 double: not needed
             dum5 = struct.unpack('d',f.read(8))[0] # 1 double: not needed
             dum6 = struct.unpack('d',f.read(8))[0] # 1 double: not needed
             dum7 = struct.unpack('d',f.read(8))[0] # 1 double: not needed
             dumIntensity = struct.unpack('d',f.read(8))[0]
             dum8 = struct.unpack('d',f.read(8))[0] # 1 double: not needed
             dum9 = struct.unpack('d',f.read(8))[0] # 1 double: not needed
             dum10 = struct.unpack('d',f.read(8))[0] # 1 double: not needed
             dum11 = struct.unpack('d',f.read(8))[0] # 1 double: not needed
             dum12 = struct.unpack('d',f.read(8))[0] # 1 double: not needed
             dum13 = struct.unpack('d',f.read(8))[0] # 1 double: not needed
             dum14 = struct.unpack('d',f.read(8))[0] # 1 double: not needed
             dum15 = struct.unpack('d',f.read(8))[0] # 1 double: not needed
             if i==(segs-1):  # only take last segment
                X.append(dumX)
                Y.append(dumY)
                Z.append(dumZ)
                Ux.append(dumUx)
                Uy.append(dumUy)
                Uz.append(dumUz)
                Intensity.append(dumIntensity)
                loops = loops + 1
        
except FileNotFoundError:
   print("Error: file not found.")

# write data
# check if 'debug' output
if (output_filename == 'debug'):
    print('DEBUG: first 5 rays')
    for i in range(0,5,1):
      print('X = ',X[i])
      print('Y = ',Y[i])
      print('Z = ',Z[i])
      print('Ux = ',Ux[i])
      print('Uy = ',Uy[i])
      print('Uz = ',Uz[i])
      print('Intensity = ',Intensity[i])
else:
    with open(output_filename,'wb') as output:
        for i in range(0,loops-1,1):
          output.write(struct.pack('d',X[i]))
          output.write(struct.pack('d',Y[i]))
          output.write(struct.pack('d',Z[i]))
          output.write(struct.pack('d',Ux[i]))
          output.write(struct.pack('d',Uy[i]))
          output.write(struct.pack('d',Uz[i]))
          output.write(struct.pack('d',Intensity[i]))
    # write associated .txt file with number of rays
    ascii_content_number = "{\n \"NumberOfElements\": " + str(loops-1) + "\n},\n"
    version = '2025'   # Ray Illumination database format version
    ascii_content_version = "{\n \"Version\": " + version + "\n}\n"
    with open(output_filename + ".txt",'w') as output_text:
        output_text.write(ascii_content_number)
        output_text.write(ascii_content_version)
    
      


