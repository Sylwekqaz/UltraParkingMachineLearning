ob = csvread('cachedFeatures.csv');
true_ob = ob(ob(:, 4) == 1, :);
false_ob = ob(ob(:, 4) == 0, :);

freecolor = [255 72 0] /255;
takencolor = [100 100 100] /255;
labels = {'','','','','Wspolczynnik krawedzi','Wspolczynnik chrominancji','Srednia saturacja', 'Odchylenie standardowe saturacji','Srednia wartosc','Odchylenie standardowe wartosci'};
for x = 5:10
  for y = x:10
  if x==y
    continue
  end
  fig = figure;
  hold on;
  scatter(true_ob(:,x) ,true_ob(:,y) ,[],takencolor,'filled')
  scatter(false_ob(:,x),false_ob(:,y),[],freecolor ,'filled')
  xlabel(labels(1,x));
  ylabel(labels(1,y));
  hold off;
  print(fig,sprintf('%d_z_%d.jpg', x,y),'-djpg','-S300,300' )
  end;
end;