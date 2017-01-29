ob = csvread('cachedFeatures.csv');
true_ob = ob(ob(:, 4) == 1, :);
false_ob = ob(ob(:, 4) == 0, :);

freecolor = [255 72 0] /255;
takencolor = [100 100 100] /255;

for var = 5:10
  minv = min(ob(:,var));
  maxv = max(ob(:,var));
  bins = linspace(minv,maxv,10);

  [n2,x2] = hist(true_ob(:,var),bins);
  [n3,x3] = hist(false_ob(:,var),bins);

  figure;
  hold on;
  
  h3 = bar(x3,n3,'facecolor',freecolor ,'BarWidth', 1,'edgecolor','none');
  h2 = bar(x2,n2,'facecolor',takencolor,'BarWidth', 1,'edgecolor','none');
  %,'facealpha', 0.80
  %,'facealpha', 0.40
  hold off;
end
