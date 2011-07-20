%UpdateDirectionAndPositionAfterGivenFlags
function polAngle = Func_UpdatePolarAngleForDirectionalSources(FullLength, CurLength, ThetaConv)

if (ThetaConv == 0.0)    
    polAngle = ThetaConv;
else
    height = FullLength / tan(ThetaConv);
    polAngle =  atan(CurLength) / height;
end

