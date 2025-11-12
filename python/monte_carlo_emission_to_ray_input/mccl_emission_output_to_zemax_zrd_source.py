# This is an example of python code to convert MCCL Output to a PhotonEmissionDatabase
# for subsequent reading into Zemax 
# This site provided helpful example code: 
# https://community.zemax.com/code-exchange-10/python-reading-writing-binary-files-zrd-zbf-dat-sdf-3116
import sys
import struct
import json
import numpy as np
# check if enough arguments are provided
if len(sys.argv) < 3:
   print("Usage: python3 mccl_emission_output_to_zemax_zrd.py <fileToConvert> <ConvertedFile>")
else:
   input_filename = sys.argv[1]
   output_filename = sys.argv[2]

# Open MCCL PhotonEmissionDatabase file
try:
   # read number of photons from JSON file
   json_data = json.loads(input_filename + '.txt')
   number_of_photons = json.data('NumberOfElements')
   with open(input_filename,'rb') as f:
      # read header
      #version = int.from_bytes(f.read(4),byteorder='little')
      #print('version = ' + str(version))
      loops = 0
      X = []      
      Y = []      
      Z = []      
      Ux = []      
      Uy = []      
      Uz = []      
      Intensity = []
      TimeInTissue = []
      while True:
         for i in range(0,number_of_photons,1):
             dumX = struct.unpack('d',f.read(8))[0]
             dumY = struct.unpack('d',f.read(8))[0]
             dumZ = struct.unpack('d',f.read(8))[0]
             dumUx = struct.unpack('d',f.read(8))[0]
             dumUy = struct.unpack('d',f.read(8))[0]
             dumUz = struct.unpack('d',f.read(8))[0]
             dumIntensity = struct.unpack('d',f.read(8))[0]
             dumTimeInTissue = struct.unpack('d',f.read(8))[0]
             print('loops = ' + str(loops))
             X.append(dumX)
             Y.append(dumY)
             Z.append(dumZ)
             Ux.append(dumUx)
             Uy.append(dumUy)
             Uz.append(dumUz)
             Intensity.append(dumIntensity)
             TimeInTissue.append(dumTimeInTissue)
             print('X = ' + str(X[loops]))
             print('Y = ' + str(Y[loops]))
             print('Z = ' + str(Z[loops]))
             print('Ux = ' + str(Ux[loops]))
             print('Uy = ' + str(Uy[loops]))
             print('Uz = ' + str(Uz[loops]))
             print('Intensity = ' + str(Intensity[loops]))
             print('TimeInTissue = ' + str(TimeInTissue[loops]))
             loops = loops + 1
        
except FileNotFoundError:
   print("Error: file not found.")

# write data
with open(output_filename,'wb') as output:
    # write header: version and max number of segments 
    version = 2002
    max_segments = 500
    output.write(struct.pack('i',version))
    output.write(struct.pack('i',max_segments))
    for i in range(0,loops-1,1):
      output.write(struct.pack('I',0)) # unsigned int status
      output.write(struct.pack('i',0)) # int level
      output.write(struct.pack('i',0)) # int hit_object
      output.write(struct.pack('i',0)) # int hit_face
      output.write(struct.pack('i',0)) # int unused 
      output.write(struct.pack('i',0)) # int in_object 
      output.write(struct.pack('i',0)) # int parent 
      output.write(struct.pack('i',0)) # int storage 
      output.write(struct.pack('i',0)) # int xybin
      output.write(struct.pack('i',0)) # int lmbin 
      output.write(struct.pack('d',0)) # double index
      output.write(struct.pack('d',0)) # double starting_phase
      output.write(struct.pack('d',X[i]))
      output.write(struct.pack('d',Y[i]))
      output.write(struct.pack('d',Z[i]))
      output.write(struct.pack('d',Ux[i]))
      output.write(struct.pack('d',Uy[i]))
      output.write(struct.pack('d',Uz[i]))
      output.write(struct.pack('d',0)) # double nx 
      output.write(struct.pack('d',0)) # double ny
      output.write(struct.pack('d',0)) # double nz
      output.write(struct.pack('d',0)) # double path_to
      output.write(struct.pack('d',Intensity[i]))
      output.write(struct.pack('d',0)) # double phase_of
      output.write(struct.pack('d',0)) # double phase_at
      output.write(struct.pack('d',0)) # double exr
      output.write(struct.pack('d',0)) # double exi
      output.write(struct.pack('d',0)) # double eyr
      output.write(struct.pack('d',0)) # double eyi
      output.write(struct.pack('d',0)) # double ezr
      output.write(struct.pack('d',0)) # double ezi

