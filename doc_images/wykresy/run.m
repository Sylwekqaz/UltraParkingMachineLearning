ob = csvread("cachedFeatures.csv");
true_ob = ob(ob(:, 4) == 1, :);
false_ob = ob(ob(:, 4) == 0, :);


for var = 5:10
  minv = min(ob(:,var));
  maxv = max(ob(:,var));
  bins = linspace(minv,maxv,25);

  [n2,x2] = hist(true_ob(:,var),bins);
  [n3,x3] = hist(false_ob(:,var),bins);

  figure;
  hold on;
  h2 = plot(x2,n2,'color',[  0.5 0.5 0.5 ]);
  h3 = plot(x3,n3,'color',[  0.5 0.5 0.5 ]);
  %,'facealpha', 0.75
  hold off;
end
